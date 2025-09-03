/*  clsCatAtonic.cs

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
*/
//
//============================================================================================//
// Dual-Licensing Statement (Applies Only to Author's Contributions, Richard Samphire MW0LGE) //
// ------------------------------------------------------------------------------------------ //
// For any code originally written by Richard Samphire MW0LGE, or for any modifications       //
// made by him, the copyright holder for those portions (Richard Samphire) reserves the       //
// right to use, license, and distribute such code under different terms, including           //
// closed-source and proprietary licences, in addition to the GNU General Public License      //
// granted above. Nothing in this statement restricts any rights granted to recipients under  //
// the GNU GPL. Code contributed by others (not Richard Samphire) remains licensed under      //
// its original terms and is not affected by this dual-licensing statement in any way.        //
// Richard Samphire can be reached by email at :  mw0lge@grange-lane.co.uk                    //
//============================================================================================//

using System;
using System.Collections.Generic;
using System.Globalization;

namespace CatAtonic
{
    public enum ScriptCommandType
    {
        CatMessage,
        CatMessageVar,
        Wait,
        CatState
    }

    public class ScriptCommand
    {
        public ScriptCommandType type;
        public string text;
        public int wait_ms;
        public string variable_name;
        public string parent_uid;
        public int macro;
    }

    public class ScriptResult
    {
        public List<ScriptCommand> commands;
        public int total_wait_ms;
        public string cat_state_command;
        public bool is_valid;
        public string error_message;

        public ScriptResult()
        {
            this.commands = new List<ScriptCommand>();
            this.total_wait_ms = 0;
            this.cat_state_command = null;
            this.is_valid = true;
            this.error_message = string.Empty;
        }
    }

    public enum TokenType
    {
        Bracket,
        Cat,
        Eof,
        Error
    }

    public class Token
    {
        public TokenType type;
        public string text;

        public Token(TokenType type, string text)
        {
            this.type = type;
            this.text = text;
        }
    }

    public class Tokeniser
    {
        private readonly string input;
        private int index;
        private readonly int length;

        public Tokeniser(string input)
        {
            this.input = input ?? string.Empty;
            this.index = 0;
            this.length = this.input.Length;
        }

        private void skip_ws_and_comments()
        {
            while (true)
            {
                while (this.index < this.length && char.IsWhiteSpace(this.input[this.index])) this.index++;
                if (this.index >= this.length) return;
                if (this.input[this.index] == '#')
                {
                    while (this.index < this.length && this.input[this.index] != '\n') this.index++;
                    if (this.index < this.length && this.input[this.index] == '\n') this.index++;
                    continue;
                }
                return;
            }
        }

        public Token next()
        {
            this.skip_ws_and_comments();
            if (this.index >= this.length) return new Token(TokenType.Eof, string.Empty);

            char c = this.input[this.index];
            if (c == '[')
            {
                int start = this.index + 1;
                int j = start;
                while (j < this.length)
                {
                    char ch = this.input[j];
                    if (ch == ']')
                    {
                        string inner = this.input.Substring(start, j - start).Trim();
                        this.index = j + 1;
                        return new Token(TokenType.Bracket, inner);
                    }
                    if (ch == '[' || ch == ';' || ch == '\r' || ch == '\n')
                    {
                        string inner_so_far = this.input.Substring(start, j - start).Trim();
                        if (inner_so_far.Length == 0) inner_so_far = "?";
                        return new Token(TokenType.Error, "non completed [] at " + inner_so_far);
                    }
                    j++;
                }
                return new Token(TokenType.Error, "non completed []");
            }
            else
            {
                int j = this.index;
                while (j < this.length)
                {
                    char ch = this.input[j];
                    if (ch == ';')
                    {
                        string cmd = this.input.Substring(this.index, j - this.index + 1).Trim();
                        this.index = j + 1;
                        return new Token(TokenType.Cat, cmd);
                    }
                    if (ch == '[' || ch == ']' || ch == '#')
                    {
                        return new Token(TokenType.Error, "non terminated cat message in a ;");
                    }
                    j++;
                }
                return new Token(TokenType.Error, "non terminated cat message in a ;");
            }
        }
    }

    public class CATScriptInterpreter
    {
        private struct if_ctx
        {
            public bool cond_true;
            public bool in_else;
            public bool seen_else;
            public bool matched;
            public HashSet<string> used_conds;
        }

        private readonly Func<int, string, bool> condition_evaluator;

        public CATScriptInterpreter(Func<int, string, bool> condition_evaluator)
        {
            this.condition_evaluator = condition_evaluator;
        }

        public ScriptResult Run(int id, string script)
        {
            ScriptResult result = new ScriptResult();
            Tokeniser t = new Tokeniser(script);
            List<if_ctx> stack = new List<if_ctx>();
            bool expect_cat_after_cat_state = false;
            bool executed_cat_state_seen = false;
            string pending_var_name = null;

            while (true)
            {
                Token tok = t.next();
                if (tok.type == TokenType.Eof) break;
                if (tok.type == TokenType.Error)
                {
                    result.is_valid = false;
                    result.error_message = tok.text;
                    return result;
                }

                if (tok.type == TokenType.Bracket)
                {
                    if (expect_cat_after_cat_state)
                    {
                        result.is_valid = false;
                        result.error_message = "STATE must be followed by a CAT command";
                        return result;
                    }

                    string inner = tok.text.Trim();
                    string upper = inner.ToUpperInvariant();

                    if (upper == "ELSE")
                    {
                        if (stack.Count == 0)
                        {
                            result.is_valid = false;
                            result.error_message = "ELSE without IF";
                            return result;
                        }
                        if_ctx ctx_else = stack[stack.Count - 1];
                        if (ctx_else.seen_else)
                        {
                            result.is_valid = false;
                            result.error_message = "multiple ELSE in IF";
                            return result;
                        }
                        ctx_else.in_else = true;
                        ctx_else.seen_else = true;
                        ctx_else.cond_true = ctx_else.matched;
                        stack[stack.Count - 1] = ctx_else;
                        continue;
                    }

                    if (upper.StartsWith("ELSE_IF_", StringComparison.Ordinal) || upper.StartsWith("ELSEIF_", StringComparison.Ordinal))
                    {
                        if (stack.Count == 0)
                        {
                            result.is_valid = false;
                            result.error_message = "ELSE_IF without IF";
                            return result;
                        }
                        if_ctx ctx_ei = stack[stack.Count - 1];
                        if (ctx_ei.seen_else)
                        {
                            result.is_valid = false;
                            result.error_message = "ELSE_IF after ELSE";
                            return result;
                        }
                        int prefix_len = upper.StartsWith("ELSEIF_", StringComparison.Ordinal) ? 7 : 8;
                        string cond_name = inner.Substring(prefix_len);
                        string cond_upper = cond_name.ToUpperInvariant();
                        if (ctx_ei.used_conds != null && ctx_ei.used_conds.Contains(cond_upper))
                        {
                            result.is_valid = false;
                            result.error_message = "duplicate condition in IF chain: " + cond_name;
                            return result;
                        }
                        if (ctx_ei.used_conds == null) ctx_ei.used_conds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        ctx_ei.used_conds.Add(cond_upper);
                        bool cond_true_ei = false;
                        if (!ctx_ei.matched)
                        {
                            if (this.condition_evaluator != null) cond_true_ei = this.condition_evaluator(id, cond_name);
                        }
                        ctx_ei.in_else = false;
                        ctx_ei.cond_true = cond_true_ei;
                        if (cond_true_ei) ctx_ei.matched = true;
                        stack[stack.Count - 1] = ctx_ei;
                        continue;
                    }

                    if (upper == "END" || upper == "ENDIF")
                    {
                        if (stack.Count == 0)
                        {
                            result.is_valid = false;
                            result.error_message = "END without IF";
                            return result;
                        }
                        stack.RemoveAt(stack.Count - 1);
                        continue;
                    }

                    if (upper == "STATE")
                    {
                        if (IsActive(stack))
                        {
                            if (executed_cat_state_seen)
                            {
                                result.is_valid = false;
                                result.error_message = "multiple STATE on active path";
                                return result;
                            }
                            expect_cat_after_cat_state = true;
                        }
                        continue;
                    }

                    if (upper.StartsWith("VAR_", StringComparison.Ordinal))
                    {
                        if (IsActive(stack))
                        {
                            if (pending_var_name != null)
                            {
                                result.is_valid = false;
                                result.error_message = "multiple VAR before CAT";
                                return result;
                            }
                            pending_var_name = inner.Substring(4);
                            if (pending_var_name.Length == 0)
                            {
                                result.is_valid = false;
                                result.error_message = "empty VAR name";
                                return result;
                            }
                        }
                        continue;
                    }

                    if (upper == "WAIT")
                    {
                        if (IsActive(stack))
                        {
                            ScriptCommand cmd_w = new ScriptCommand();
                            cmd_w.type = ScriptCommandType.Wait;
                            cmd_w.text = "[WAIT]";
                            cmd_w.wait_ms = 100;
                            cmd_w.variable_name = null;
                            result.commands.Add(cmd_w);
                            result.total_wait_ms += 100;
                        }
                        continue;
                    }

                    if (upper.StartsWith("WAIT", StringComparison.Ordinal))
                    {
                        string digits = inner.Substring(4);
                        int ms = 0;
                        bool ok = int.TryParse(digits, NumberStyles.Integer, CultureInfo.InvariantCulture, out ms);
                        if (!ok || ms < 0)
                        {
                            result.is_valid = false;
                            result.error_message = "invalid WAIT value";
                            return result;
                        }
                        if (IsActive(stack))
                        {
                            ScriptCommand cmd_wv = new ScriptCommand();
                            cmd_wv.type = ScriptCommandType.Wait;
                            cmd_wv.text = "[WAIT" + ms.ToString(CultureInfo.InvariantCulture) + "]";
                            cmd_wv.wait_ms = ms;
                            cmd_wv.variable_name = null;
                            result.commands.Add(cmd_wv);
                            result.total_wait_ms += ms;
                        }
                        continue;
                    }

                    if (upper.StartsWith("IF_", StringComparison.Ordinal))
                    {
                        string cond_name_if = inner.Substring(3);
                        string cond_upper_if = cond_name_if.ToUpperInvariant();
                        bool cond_true_if = false;
                        if (this.condition_evaluator != null) cond_true_if = this.condition_evaluator(id, cond_name_if);
                        if_ctx ctx_if = new if_ctx();
                        ctx_if.cond_true = cond_true_if;
                        ctx_if.in_else = false;
                        ctx_if.seen_else = false;
                        ctx_if.matched = cond_true_if;
                        ctx_if.used_conds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        ctx_if.used_conds.Add(cond_upper_if);
                        stack.Add(ctx_if);
                        continue;
                    }

                    result.is_valid = false;
                    result.error_message = "unknown command in []: " + inner;
                    return result;
                }

                if (tok.type == TokenType.Cat)
                {
                    string cmd_text = tok.text;

                    if (expect_cat_after_cat_state)
                    {
                        ScriptCommand cmd_cs = new ScriptCommand();
                        cmd_cs.type = ScriptCommandType.CatState;
                        cmd_cs.text = cmd_text;
                        cmd_cs.wait_ms = 0;
                        cmd_cs.variable_name = pending_var_name;
                        result.commands.Add(cmd_cs);

                        result.cat_state_command = cmd_text;
                        expect_cat_after_cat_state = false;
                        executed_cat_state_seen = true;
                        pending_var_name = null;
                        continue;
                    }

                    if (IsActive(stack))
                    {
                        ScriptCommand cmd = new ScriptCommand();
                        if (pending_var_name != null)
                        {
                            cmd.type = ScriptCommandType.CatMessageVar;
                            cmd.text = cmd_text;
                            cmd.wait_ms = 0;
                            cmd.variable_name = pending_var_name;
                            pending_var_name = null;
                        }
                        else
                        {
                            cmd.type = ScriptCommandType.CatMessage;
                            cmd.text = cmd_text;
                            cmd.wait_ms = 0;
                            cmd.variable_name = null;
                        }
                        result.commands.Add(cmd);
                    }

                    continue;
                }
            }

            if (stack.Count != 0)
            {
                result.is_valid = false;
                result.error_message = "missing [END] to a starting [IF]";
                return result;
            }

            if (expect_cat_after_cat_state)
            {
                result.is_valid = false;
                result.error_message = "STATE without following CAT command";
                return result;
            }

            if (pending_var_name != null)
            {
                result.is_valid = false;
                result.error_message = "VAR without following CAT command";
                return result;
            }

            return result;
        }

        public bool Validate(int id, string script)
        {
            ScriptResult r = Run(id, script);
            return r.is_valid;
        }

        public string Cat_state_command(int id, string script)
        {
            ScriptResult r = Run(id, script);
            if (!r.is_valid) return string.Empty;
            return r.cat_state_command ?? string.Empty;
        }

        public int Total_wait_milliseconds(int id, string script)
        {
            ScriptResult r = Run(id, script);
            if (!r.is_valid) return 0;
            return r.total_wait_ms;
        }

        private bool IsActive(List<if_ctx> stack)
        {
            bool active = true;
            int k = 0;
            int count = stack.Count;
            while (k < count)
            {
                if_ctx ctx = stack[k];
                bool branch = ctx.in_else ? !ctx.cond_true : ctx.cond_true;
                if (!branch)
                {
                    active = false;
                    break;
                }
                k++;
            }
            return active;
        }
    }
}
