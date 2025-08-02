/*  rnnr.c

This file is part of a program that implements a Software-Defined Radio.

This code/file can be found on GitHub : https://github.com/ramdor/Thetis

Copyright (C) 2020-2025 Richard Samphire MW0LGE

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

The author can be reached by email at

mw0lge@grange-lane.co.uk

This code is based on code and ideas from  : https://github.com/vu3rdd/wdsp
and and uses RNNoise and libspecbleach
https://gitlab.xiph.org/xiph/rnnoise
https://github.com/lucianodato/libspecbleach

It uses a non modified version of rmnoise and implements a ringbuffer to handle input/output frame size differences.
*/
 
#define _CRT_SECURE_NO_WARNINGS
#include "comm.h"
#include "rnnoise.h"

//ringbuffer
static void ring_buffer_init(rnnr_ring_buffer* rb, int capacity) 
{
    rb->buf = malloc0(capacity * sizeof(float));
    rb->capacity = capacity;
    rb->head = 0;
    rb->tail = 0;
    rb->count = 0;
}

static void ring_buffer_free(rnnr_ring_buffer* rb) 
{
    _aligned_free(rb->buf);
    rb->buf = NULL;
    rb->capacity = 0;
    rb->head = rb->tail = rb->count = 0;
}

static void ring_buffer_put(rnnr_ring_buffer* rb, float v) 
{
    if (rb->count < rb->capacity) 
    {
        rb->buf[rb->tail] = v;
        rb->tail = (rb->tail + 1) % rb->capacity;
        rb->count++;
    }
}

static int ring_buffer_get_bulk(rnnr_ring_buffer* rb, float* dest, int n)
{
    int to_get = n < rb->count ? n : rb->count;
    for (int i = 0; i < to_get; i++)
    {
        dest[i] = rb->buf[rb->head];
        rb->head = (rb->head + 1) % rb->capacity;
    }
    rb->count -= to_get;
    return to_get;
}

static void ring_buffer_resize(rnnr_ring_buffer* rb, int new_capacity)
{
    if (new_capacity == rb->capacity) return;
    float* new_buf = malloc0(new_capacity * sizeof(float));
    int cnt = rb->count;
    for (int i = 0; i < cnt; i++)
    {
        new_buf[i] = rb->buf[(rb->head + i) % rb->capacity];
    }
    _aligned_free(rb->buf);
    rb->buf = new_buf;
    rb->capacity = new_capacity;
    rb->head = 0;
    rb->tail = cnt % new_capacity;
}
//

PORT
void SetRXARNNRRun (int channel, int run)
{
	RNNR a = rxa[channel].rnnr.p;
	if (a->run != run)
	{
		RXAbp1Check (channel, rxa[channel].amd.p->run, rxa[channel].snba.p->run, 
                             rxa[channel].emnr.p->run, rxa[channel].anf.p->run, rxa[channel].anr.p->run,
                             run, rxa[channel].sbnr.p->run); // NR3 + NR4 support

		EnterCriticalSection (&ch[channel].csDSP);
		a->run = run;
		RXAbp1Set (channel);
		LeaveCriticalSection (&ch[channel].csDSP);
	}
}

void setSize_rnnr(RNNR a, int size) 
{
    _aligned_free(a->output_buffer);
    a->buffer_size = size;
    a->output_buffer = malloc0(a->buffer_size * sizeof(float));

    int new_cap = a->frame_size + a->buffer_size;
    ring_buffer_resize(&a->input_ring, new_cap);
    ring_buffer_resize(&a->output_ring, new_cap);
}

void setBuffers_rnnr(RNNR a, double* in, double* out)
{
	a->in = in;
	a->out = out;
}

void setSamplerate_rnnr(RNNR a, int rate)
{
    a->rate = rate; // not used currently, but here for future use
}

RNNR create_rnnr(int run, int position, int size, double* in, double* out, int rate)
{
    RNNR a = malloc0(sizeof(rnnr));
    a->run = run;
    a->position = position;
    a->rate = rate; // not used currently, but here for future use
    a->st = rnnoise_create(NULL);
    a->frame_size = rnnoise_get_frame_size();
    a->in = in;
    a->out = out;
    a->buffer_size = size;
    a->gain = 32767.0; //scale to 16bit for RNnoise //24bit would be 8388607.0; // note rnnoise is pcm mono 16bit

    ring_buffer_init(&a->input_ring, a->frame_size + a->buffer_size);
    ring_buffer_init(&a->output_ring, a->frame_size + a->buffer_size);
    a->to_process_buffer = malloc0(a->frame_size * sizeof(float));
    a->processed_output_buffer = malloc0(a->frame_size * sizeof(float));
    a->output_buffer = malloc0(a->buffer_size * sizeof(float));

    return a;
}

void xrnnr(RNNR a, int pos) 
{
    if (a->run && pos == a->position) 
    {
        int  bs = a->buffer_size;
        int  fs = a->frame_size;
        float* to_proc = a->to_process_buffer;
        float* proc_out = a->processed_output_buffer;

        for (int i = 0; i < bs; i++) 
        {
            ring_buffer_put(&a->input_ring, (float)a->in[2 * i] * a->gain);
            if (a->input_ring.count >= fs)
            {
                ring_buffer_get_bulk(&a->input_ring, to_proc, fs);
                rnnoise_process_frame(a->st, proc_out, to_proc);
                for (int j = 0; j < fs; j++)
                {
                    ring_buffer_put(&a->output_ring, proc_out[j] / a->gain);
                }
            }
        }
        if (a->output_ring.count >= bs) 
        {
            ring_buffer_get_bulk(&a->output_ring, a->output_buffer, bs);
            for (int i = 0; i < bs; i++) 
            {
                a->out[2 * i] = a->output_buffer[i];
                a->out[2 * i + 1] = 0;
            }
        }
        else 
        {
            memcpy(a->out, a->in, a->buffer_size * sizeof * a->out);
        }
    }
    else if (a->out != a->in) 
    {
        memcpy(a->out, a->in, a->buffer_size * sizeof * a->out);
    }
}

void destroy_rnnr(RNNR a) 
{
    rnnoise_destroy(a->st);
    _aligned_free(a->to_process_buffer);
    _aligned_free(a->processed_output_buffer);
    _aligned_free(a->output_buffer);
    ring_buffer_free(&a->input_ring);
    ring_buffer_free(&a->output_ring);
    _aligned_free(a);
}

PORT
void SetRXARNNRgain(int channel, float gain)
{
    if (gain <= 0) return;

    EnterCriticalSection(&ch[channel].csDSP);
    rxa[channel].rnnr.p->gain = gain;
    LeaveCriticalSection(&ch[channel].csDSP);
}
