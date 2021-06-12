/*----------------------------------------------------------------------
Compiler Generator Coco/R,
Copyright (c) 1990, 2004 Hanspeter Moessenboeck, University of Linz
extended by M. Loeberbauer & A. Woess, Univ. of Linz
with improvements by Pat Terry, Rhodes University

This program is free software; you can redistribute it and/or modify it
under the terms of the GNU General Public License as published by the
Free Software Foundation; either version 2, or (at your option) any
later version.

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
for more details.

You should have received a copy of the GNU General Public License along
with this program; if not, write to the Free Software Foundation, Inc.,
59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.

As an exception, it is allowed to write an extension of Coco/R that is
used as a plugin in non-free software.

If not otherwise stated, any source code generated by Coco/R (other than
Coco/R itself) does not fall under the GNU General Public License.
-----------------------------------------------------------------------*/
using System.IO;



using System;
using System.Collections;

namespace at.jku.ssw.Coco {



#if PARSER_WITH_AST
public class SynTree {
	public SynTree(Token t ) {
		tok = t;
		children = new ArrayList();
	}

	public Token tok;
	public ArrayList children;

	static void printIndent(int n) {
		for(int i=0; i < n; ++i) Console.Write(" ");
	}

	public void dump(int indent=0, bool isLast=false) {
        int last_idx = children.Count;
        if(tok.col > 0) {
            printIndent(indent);
            Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", ((isLast || (last_idx == 0)) ? "= " : " "), tok.line, tok.col, tok.kind, tok.val);
        }
        else {
            printIndent(indent);
            Console.WriteLine("{0}\t{1}\t{2}\t{3}", children.Count, tok.line, tok.kind, tok.val);
        }
        if(last_idx > 0) {
                for(int idx=0; idx < last_idx; ++idx) ((SynTree)children[idx]).dump(indent+4, idx == last_idx);
        }
	}

	public void dump2(int indent=0, bool isLast=false) {
        int last_idx = children.Count;
        int indentPlus = 4;
        if(tok.col > 0) {
            printIndent(indent);
            Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", ((isLast || (last_idx == 0)) ? "= " : " "), tok.line, tok.col, tok.kind, tok.val);
        }
        else {
            if(last_idx == 1) {
                if(((SynTree)children[0]).cildren.Count == 0) {
                    printIndent(indent);
                    Console.WriteLine("{0}\t{1}\t{2}\t{3}", children.Count, tok.line, tok.kind, tok.val);
                }
                else indentPlus = 0;
            }
            else {
                printIndent(indent);
                Console.WriteLine("{0}\t{1}\t{2}\t{3}", children.Count, tok.line, tok.kind, tok.val);
            }
        }
        if(last_idx > 0) {
                for(int idx=0; idx < last_idx; ++idx) ((SynTree)children[idx]).dump2(indent+indentPlus, idx == last_idx);
        }
	}
};
#endif

public class Parser {
	//non terminals
	public const int _NT_Coco = 0;
	public const int _NT_SetDecl = 1;
	public const int _NT_TokenDecl = 2;
	public const int _NT_TokenExpr = 3;
	public const int _NT_Set = 4;
	public const int _NT_AttrDecl = 5;
	public const int _NT_SemText = 6;
	public const int _NT_Expression = 7;
	public const int _NT_SimSet = 8;
	public const int _NT_Char = 9;
	public const int _NT_Sym = 10;
	public const int _NT_Term = 11;
	public const int _NT_Resolver = 12;
	public const int _NT_Factor = 13;
	public const int _NT_Attribs = 14;
	public const int _NT_Condition = 15;
	public const int _NT_TokenTerm = 16;
	public const int _NT_TokenFactor = 17;
	public const int maxNT = 17;
	//terminals
	public const int _EOF = 0;
	public const int _ident = 1;
	public const int _number = 2;
	public const int _string = 3;
	public const int _badString = 4;
	public const int _char = 5;
//	public const int _("COMPILER") = 6;
//	public const int _("IGNORECASE") = 7;
//	public const int _("TERMINALS") = 8;
//	public const int _("CHARACTERS") = 9;
//	public const int _("TOKENS") = 10;
//	public const int _("PRAGMAS") = 11;
//	public const int _("COMMENTS") = 12;
//	public const int _("FROM") = 13;
//	public const int _("TO") = 14;
//	public const int _("NESTED") = 15;
//	public const int _("IGNORE") = 16;
//	public const int _("PRODUCTIONS") = 17;
//	public const int _("=") = 18;
//	public const int _(".") = 19;
//	public const int _("END") = 20;
//	public const int _("+") = 21;
//	public const int _("-") = 22;
//	public const int _("..") = 23;
//	public const int _("ANY") = 24;
//	public const int _("<") = 25;
//	public const int _(">") = 26;
//	public const int _("<.") = 27;
//	public const int _(".>") = 28;
//	public const int _("|") = 29;
//	public const int _("WEAK") = 30;
//	public const int _("(") = 31;
//	public const int _(")") = 32;
//	public const int _("[") = 33;
//	public const int _("]") = 34;
//	public const int _("{") = 35;
//	public const int _("}") = 36;
//	public const int _("SYNC") = 37;
//	public const int _("IF") = 38;
//	public const int _("CONTEXT") = 39;
//	public const int _("(.") = 40;
//	public const int _(".)") = 41;
//	public const int _(???) = 42;
	public const int maxT = 42;
	public const int _ddtSym = 43;
	public const int _optionSym = 44;

	const bool _T = true;
	const bool _x = false;
	const int minErrDist = 2;

	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

const int id = 0;
	const int str = 1;

	public TextWriter trace;    // other Coco objects referenced in this ATG
	public Tab tab;
	public DFA dfa;
	public ParserGen pgen;

	bool   genScanner;
	bool ignoreGammarErrors = false;
	string tokenString;         // used in declarations of literal tokens
	string noString = "-none-"; // used in declarations of literal tokens
	string gramName; // grammar name

/*-------------------------------------------------------------------------*/



	public Parser(Scanner scanner) {
		this.scanner = scanner;
		errors = new Errors();
	}

	void SynErr (int n) {
		if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}

	public void SemErr (string msg) {
		if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
		errDist = 0;
	}

	void Get () {
		for (;;) {
			t = la;
			la = scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }
			if (la.kind == _ddtSym) {
				tab.SetDDT(la.val); 
			}
			if (la.kind == _optionSym) {
				tab.SetOption(la.val); 
			}

			la = t;
		}
	}

	void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}

	bool StartOf (int s) {
		return set[s, la.kind];
	}

	void ExpectWeak (int n, int follow) {
		if (la.kind == n) Get();
		else {
			SynErr(n);
			while (!StartOf(follow)) Get();
		}
	}


	bool WeakSeparator(int n, int syFol, int repFol) {
		int kind = la.kind;
		if (kind == n) {Get(); return true;}
		else if (StartOf(repFol)) {return false;}
		else {
			SynErr(n);
			while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) {
				Get();
				kind = la.kind;
			}
			return StartOf(syFol);
		}
	}


	void Coco() {
		Symbol sym; Graph g, g1, g2; CharSet s; int beg, line; 
		if (StartOf(1 /* any   */)) {
			Get();
			beg = t.pos; line = t.line; 
			while (StartOf(1 /* any   */)) {
				Get();
			}
			pgen.usingPos = new Position(beg, la.pos, 0, line); 
		}
		Expect(6 /* "COMPILER" */);
		genScanner = true;
		tab.ignored = new CharSet(); 
		Expect(_ident);
		gramName = t.val;
		beg = la.pos; line = la.line;
		
		while (StartOf(2 /* any   */)) {
			Get();
		}
		tab.semDeclPos = new Position(beg, la.pos, 0, line); 
		if (la.kind == 7 /* "IGNORECASE" */) {
			Get();
			dfa.ignoreCase = true; 
		}
		if (la.kind == 8 /* "TERMINALS" */) {
			Get();
			while (la.kind == _ident) {
				Get();
				sym = tab.FindSym(t.val);
				if (sym != null) SemErr("name declared twice");
				else {
				sym = tab.NewSym(Node.t, t.val, t.line, t.col);
				sym.tokenKind = Symbol.fixedToken;
				}
			}
		}
		if (la.kind == 9 /* "CHARACTERS" */) {
			Get();
			while (la.kind == _ident) {
				SetDecl();
			}
		}
		if (la.kind == 10 /* "TOKENS" */) {
			Get();
			while (la.kind == _ident || la.kind == _string || la.kind == _char) {
				TokenDecl(Node.t);
			}
		}
		if (la.kind == 11 /* "PRAGMAS" */) {
			Get();
			while (la.kind == _ident || la.kind == _string || la.kind == _char) {
				TokenDecl(Node.pr);
			}
		}
		while (la.kind == 12 /* "COMMENTS" */) {
			Get();
			bool nested = false; 
			Expect(13 /* "FROM" */);
			TokenExpr(out g1);
			Expect(14 /* "TO" */);
			TokenExpr(out g2);
			if (la.kind == 15 /* "NESTED" */) {
				Get();
				nested = true; 
			}
			dfa.NewComment(g1.l, g2.l, nested); 
		}
		while (la.kind == 16 /* "IGNORE" */) {
			Get();
			Set(out s);
			tab.ignored.Or(s); 
		}
		while (!(la.kind == _EOF || la.kind == 17 /* "PRODUCTIONS" */)) {SynErr(43); Get();}
		Expect(17 /* "PRODUCTIONS" */);
		if (genScanner) dfa.MakeDeterministic();
		tab.DeleteNodes();
		
		while (la.kind == _ident) {
			Get();
			sym = tab.FindSym(t.val);
			bool undef = sym == null;
			if (undef) sym = tab.NewSym(Node.nt, t.val, t.line, t.col);
			else {
			 if (sym.typ == Node.nt) {
			   if (sym.graph != null) SemErr("name declared twice");
			 } else SemErr("this symbol kind not allowed on left side of production");
			 sym.line = t.line;
			}
			bool noAttrs = sym.attrPos == null;
			sym.attrPos = null;
			
			if (la.kind == 25 /* "<" */ || la.kind == 27 /* "<." */) {
				AttrDecl(sym);
			}
			if (!undef)
			 if (noAttrs != (sym.attrPos == null))
			   SemErr("attribute mismatch between declaration and use of this symbol");
			
			if (la.kind == 40 /* "(." */) {
				SemText(out sym.semPos);
			}
			ExpectWeak(18 /* "=" */, 3);
			Expression(out g);
			sym.graph = g.l;
			tab.Finish(g);
			
			ExpectWeak(19 /* "." */, 4);
		}
		Expect(20 /* "END" */);
		Expect(_ident);
		if (gramName != t.val)
		 SemErr("name does not match grammar name");
		tab.gramSy = tab.FindSym(gramName);
		if (tab.gramSy == null)
		 SemErr("missing production for grammar name");
		else {
		 sym = tab.gramSy;
		 if (sym.attrPos != null)
		   SemErr("grammar symbol must not have attributes");
		}
		tab.noSym = tab.NewSym(Node.t, "???", 0, 0); // noSym gets highest number
		tab.SetupAnys();
		tab.RenumberPragmas();
		if (tab.ddt[2]) tab.PrintNodes();
		if (errors.count == 0) {
		 Console.WriteLine("checking");
		 tab.CompSymbolSets();
		 if (tab.ddt[7]) tab.XRef();
		 bool doGenCode = false;
		 if(ignoreGammarErrors) {
		   doGenCode = true;
		   tab.GrammarCheckAll();
		 }
		 else doGenCode = tab.GrammarOk();
		 if(tab.genRREBNF && doGenCode) {
		   pgen.WriteRREBNF();
		 }
		 if (doGenCode) {
		   Console.Write("parser");
		   pgen.WriteParser();
		   if (genScanner) {
		     Console.Write(" + scanner");
		     dfa.WriteScanner();
		     if (tab.ddt[0]) dfa.PrintStates();
		   }
		   Console.WriteLine(" generated");
		   if (tab.ddt[8]) pgen.WriteStatistics();
		 }
		}
		if (tab.ddt[6]) tab.PrintSymbolTable();
		
		Expect(19 /* "." */);
	}

	void SetDecl() {
		CharSet s; 
		Expect(_ident);
		string name = t.val;
		CharClass c = tab.FindCharClass(name);
		if (c != null) SemErr("name declared twice");
		
		Expect(18 /* "=" */);
		Set(out s);
		if (s.Elements() == 0) SemErr("character set must not be empty");
		tab.NewCharClass(name, s);
		
		Expect(19 /* "." */);
	}

	void TokenDecl(int typ) {
		string name; int kind; Symbol sym; Graph g; 
		Sym(out name, out kind);
		sym = tab.FindSym(name);
		if (sym != null) SemErr("name declared twice");
		else {
		 sym = tab.NewSym(typ, name, t.line, t.col);
		 sym.tokenKind = Symbol.fixedToken;
		}
		tokenString = null;
		
		while (!(StartOf(5 /* sync  */))) {SynErr(44); Get();}
		if (la.kind == 18 /* "=" */) {
			Get();
			TokenExpr(out g);
			Expect(19 /* "." */);
			if (kind == str) SemErr("a literal must not be declared with a structure");
			tab.Finish(g);
			if (tokenString == null || tokenString.Equals(noString))
			 dfa.ConvertToStates(g.l, sym);
			else { // TokenExpr is a single string
			 if (tab.literals[tokenString] != null)
			   SemErr("token string declared twice");
			 tab.literals[tokenString] = sym;
			 dfa.MatchLiteral(tokenString, sym);
			}
			
		} else if (StartOf(6 /* sem   */)) {
			if (kind == id) genScanner = false;
			else dfa.MatchLiteral(sym.name, sym);
			
		} else SynErr(45);
		if (la.kind == 40 /* "(." */) {
			SemText(out sym.semPos);
			if (typ == Node.t) errors.Warning("Warning semantic action on token declarations require a custom Scanner"); 
		}
	}

	void TokenExpr(out Graph g) {
		Graph g2; 
		TokenTerm(out g);
		bool first = true; 
		while (WeakSeparator(29 /* "|" */,7,8) ) {
			TokenTerm(out g2);
			if (first) { tab.MakeFirstAlt(g); first = false; }
			tab.MakeAlternative(g, g2);
			
		}
	}

	void Set(out CharSet s) {
		CharSet s2; 
		SimSet(out s);
		while (la.kind == 21 /* "+" */ || la.kind == 22 /* "-" */) {
			if (la.kind == 21 /* "+" */) {
				Get();
				SimSet(out s2);
				s.Or(s2); 
			} else {
				Get();
				SimSet(out s2);
				s.Subtract(s2); 
			}
		}
	}

	void AttrDecl(Symbol sym) {
		if (la.kind == 25 /* "<" */) {
			Get();
			int beg = la.pos; int col = la.col; int line = la.line; 
			while (StartOf(9 /* alt   */)) {
				if (StartOf(10 /* any   */)) {
					Get();
				} else {
					Get();
					SemErr("bad string in attributes"); 
				}
			}
			Expect(26 /* ">" */);
			if (t.pos > beg)
			 sym.attrPos = new Position(beg, t.pos, col, line); 
		} else if (la.kind == 27 /* "<." */) {
			Get();
			int beg = la.pos; int col = la.col; int line = la.line; 
			while (StartOf(11 /* alt   */)) {
				if (StartOf(12 /* any   */)) {
					Get();
				} else {
					Get();
					SemErr("bad string in attributes"); 
				}
			}
			Expect(28 /* ".>" */);
			if (t.pos > beg)
			 sym.attrPos = new Position(beg, t.pos, col, line); 
		} else SynErr(46);
	}

	void SemText(out Position pos) {
		Expect(40 /* "(." */);
		int beg = la.pos; int col = la.col; int line = la.line; 
		while (StartOf(13 /* alt   */)) {
			if (StartOf(14 /* any   */)) {
				Get();
			} else if (la.kind == _badString) {
				Get();
				SemErr("bad string in semantic action"); 
			} else {
				Get();
				SemErr("missing end of previous semantic action"); 
			}
		}
		Expect(41 /* ".)" */);
		pos = new Position(beg, t.pos, col, line); 
	}

	void Expression(out Graph g) {
		Graph g2; 
		Term(out g);
		bool first = true; 
		while (WeakSeparator(29 /* "|" */,15,16) ) {
			Term(out g2);
			if (first) { tab.MakeFirstAlt(g); first = false; }
			tab.MakeAlternative(g, g2);
			
		}
	}

	void SimSet(out CharSet s) {
		int n1, n2; 
		s = new CharSet(); 
		if (la.kind == _ident) {
			Get();
			CharClass c = tab.FindCharClass(t.val);
			if (c == null) SemErr("undefined name"); else s.Or(c.set);
			
		} else if (la.kind == _string) {
			Get();
			string name = tab.Unstring(t.val);
			foreach (char ch in name)
			 if (dfa.ignoreCase) s.Set(char.ToLower(ch));
			 else s.Set(ch); 
		} else if (la.kind == _char) {
			Char(out n1);
			s.Set(n1); 
			if (la.kind == 23 /* ".." */) {
				Get();
				Char(out n2);
				for (int i = n1; i <= n2; i++) s.Set(i); 
			}
		} else if (la.kind == 24 /* "ANY" */) {
			Get();
			s = new CharSet(); s.Fill(); 
		} else SynErr(47);
	}

	void Char(out int n) {
		Expect(_char);
		string name = tab.Unstring(t.val); n = 0;
		if (name.Length == 1) n = name[0];
		else SemErr("unacceptable character value");
		if (dfa.ignoreCase && (char)n >= 'A' && (char)n <= 'Z') n += 32;
		
	}

	void Sym(out string name, out int kind) {
		name = "???"; kind = id; 
		if (la.kind == _ident) {
			Get();
			kind = id; name = t.val; 
		} else if (la.kind == _string || la.kind == _char) {
			if (la.kind == _string) {
				Get();
				name = t.val; 
			} else {
				Get();
				name = "\"" + t.val.Substring(1, t.val.Length-2) + "\""; 
			}
			kind = str;
			if (dfa.ignoreCase) name = name.ToLower();
			if (name.IndexOf(' ') >= 0)
			 SemErr("literal tokens must not contain blanks"); 
		} else SynErr(48);
	}

	void Term(out Graph g) {
		Graph g2; Node rslv = null; g = null; 
		if (StartOf(17 /* opt   */)) {
			if (la.kind == 38 /* "IF" */) {
				rslv = tab.NewNode(Node.rslv, null, la.line, la.col); 
				Resolver(out rslv.pos);
				g = new Graph(rslv); 
			}
			Factor(out g2);
			if (rslv != null) tab.MakeSequence(g, g2);
			else g = g2;
			
			while (StartOf(18 /* nt   Factor */)) {
				Factor(out g2);
				tab.MakeSequence(g, g2); 
			}
		} else if (StartOf(19 /* sem   */)) {
			g = new Graph(tab.NewNode(Node.eps, null, t.line, t.col)); 
		} else SynErr(49);
		if (g == null) // invalid start of Term
		 g = new Graph(tab.NewNode(Node.eps, null, t.line, t.col));
		
	}

	void Resolver(out Position pos) {
		Expect(38 /* "IF" */);
		Expect(31 /* "(" */);
		int beg = la.pos; int col = la.col; int line = la.line; 
		Condition();
		pos = new Position(beg, t.pos, col, line); 
	}

	void Factor(out Graph g) {
		string name; int kind; Position pos; bool weak = false;
		g = null;
		
		switch (la.kind) {
		case _ident: case _string: case _char: case 30 /* "WEAK" */: {
			if (la.kind == 30 /* "WEAK" */) {
				Get();
				weak = true; 
			}
			Sym(out name, out kind);
			Symbol sym = tab.FindSym(name);
			if (sym == null && kind == str)
			 sym = tab.literals[name] as Symbol;
			bool undef = sym == null;
			if (undef) {
			 if (kind == id)
			   sym = tab.NewSym(Node.nt, name, 0, 0);  // forward nt
			 else if (genScanner) {
			   sym = tab.NewSym(Node.t, name, t.line, t.col);
			   dfa.MatchLiteral(sym.name, sym);
			 } else {  // undefined string in production
			   SemErr("undefined string in production");
			   sym = tab.eofSy;  // dummy
			 }
			}
			int typ = sym.typ;
			if (typ != Node.t && typ != Node.nt)
			 SemErr("this symbol kind is not allowed in a production");
			if (weak)
			 if (typ == Node.t) typ = Node.wt;
			 else SemErr("only terminals may be weak");
			Node p = tab.NewNode(typ, sym, t.line, t.col);
			g = new Graph(p);
			
			if (la.kind == 25 /* "<" */ || la.kind == 27 /* "<." */) {
				Attribs(p);
				if (kind != id) SemErr("a literal must not have attributes"); 
			}
			if (undef)
			 sym.attrPos = p.pos;  // dummy
			else if ((p.pos == null) != (sym.attrPos == null))
			 SemErr("attribute mismatch between declaration and use of this symbol");
			
			break;
		}
		case 31 /* "(" */: {
			Get();
			Expression(out g);
			Expect(32 /* ")" */);
			break;
		}
		case 33 /* "[" */: {
			Get();
			Expression(out g);
			Expect(34 /* "]" */);
			tab.MakeOption(g); 
			break;
		}
		case 35 /* "{" */: {
			Get();
			Expression(out g);
			Expect(36 /* "}" */);
			tab.MakeIteration(g); 
			break;
		}
		case 40 /* "(." */: {
			SemText(out pos);
			Node p = tab.NewNode(Node.sem, null, t.line, t.col);
			p.pos = pos;
			g = new Graph(p);
			
			break;
		}
		case 24 /* "ANY" */: {
			Get();
			Node p = tab.NewNode(Node.any, null, t.line, t.col);  // p.set is set in tab.SetupAnys
			g = new Graph(p);
			
			break;
		}
		case 37 /* "SYNC" */: {
			Get();
			Node p = tab.NewNode(Node.sync, null, t.line, t.col);
			g = new Graph(p);
			
			break;
		}
		default: SynErr(50); break;
		}
		if (g == null) // invalid start of Factor
		 g = new Graph(tab.NewNode(Node.eps, null, t.line, t.col));
		
	}

	void Attribs(Node p) {
		if (la.kind == 25 /* "<" */) {
			Get();
			int beg = la.pos; int col = la.col; int line = la.line; 
			while (StartOf(9 /* alt   */)) {
				if (StartOf(10 /* any   */)) {
					Get();
				} else {
					Get();
					SemErr("bad string in attributes"); 
				}
			}
			Expect(26 /* ">" */);
			if (t.pos > beg) p.pos = new Position(beg, t.pos, col, line); 
		} else if (la.kind == 27 /* "<." */) {
			Get();
			int beg = la.pos; int col = la.col; int line = la.line; 
			while (StartOf(11 /* alt   */)) {
				if (StartOf(12 /* any   */)) {
					Get();
				} else {
					Get();
					SemErr("bad string in attributes"); 
				}
			}
			Expect(28 /* ".>" */);
			if (t.pos > beg) p.pos = new Position(beg, t.pos, col, line); 
		} else SynErr(51);
	}

	void Condition() {
		while (StartOf(20 /* alt   */)) {
			if (la.kind == 31 /* "(" */) {
				Get();
				Condition();
			} else {
				Get();
			}
		}
		Expect(32 /* ")" */);
	}

	void TokenTerm(out Graph g) {
		Graph g2; 
		TokenFactor(out g);
		while (StartOf(7 /* nt   TokenFactor */)) {
			TokenFactor(out g2);
			tab.MakeSequence(g, g2); 
		}
		if (la.kind == 39 /* "CONTEXT" */) {
			Get();
			Expect(31 /* "(" */);
			TokenExpr(out g2);
			tab.SetContextTrans(g2.l); dfa.hasCtxMoves = true;
			tab.MakeSequence(g, g2); 
			Expect(32 /* ")" */);
		}
	}

	void TokenFactor(out Graph g) {
		string name; int kind; 
		g = null; 
		if (la.kind == _ident || la.kind == _string || la.kind == _char) {
			Sym(out name, out kind);
			if (kind == id) {
			 CharClass c = tab.FindCharClass(name);
			 if (c == null) {
			   SemErr("undefined name: " + name);
			   c = tab.NewCharClass(name, new CharSet());
			 }
			 Node p = tab.NewNode(Node.clas, null, t.line, t.col); p.val = c.n;
			 g = new Graph(p);
			 tokenString = noString;
			} else { // str
			 g = tab.StrToGraph(name);
			 if (tokenString == null) tokenString = name;
			 else tokenString = noString;
			}
			
		} else if (la.kind == 31 /* "(" */) {
			Get();
			TokenExpr(out g);
			Expect(32 /* ")" */);
		} else if (la.kind == 33 /* "[" */) {
			Get();
			TokenExpr(out g);
			Expect(34 /* "]" */);
			tab.MakeOption(g); tokenString = noString; 
		} else if (la.kind == 35 /* "{" */) {
			Get();
			TokenExpr(out g);
			Expect(36 /* "}" */);
			tab.MakeIteration(g); tokenString = noString; 
		} else SynErr(52);
		if (g == null) // invalid start of TokenFactor
		 g = new Graph(tab.NewNode(Node.eps, null, t.line, t.col)); 
	}



	public void Parse() {
		la = new Token();
		la.val = "";
		Get();
		Coco();
		Expect(0);

	}

	static readonly bool[,] set = {
		{_T,_T,_x,_T, _x,_T,_x,_x, _x,_x,_x,_T, _T,_x,_x,_x, _T,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_x,_x,_x},
		{_x,_T,_T,_T, _T,_T,_x,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_x},
		{_x,_T,_T,_T, _T,_T,_T,_x, _x,_x,_x,_x, _x,_T,_T,_T, _x,_x,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_x},
		{_T,_T,_x,_T, _x,_T,_x,_x, _x,_x,_x,_T, _T,_x,_x,_x, _T,_T,_T,_T, _x,_x,_x,_x, _T,_x,_x,_x, _x,_T,_T,_T, _x,_T,_x,_T, _x,_T,_T,_x, _T,_x,_x,_x},
		{_T,_T,_x,_T, _x,_T,_x,_x, _x,_x,_x,_T, _T,_x,_x,_x, _T,_T,_T,_x, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_x,_x,_x},
		{_T,_T,_x,_T, _x,_T,_x,_x, _x,_x,_x,_T, _T,_x,_x,_x, _T,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_x,_x,_x},
		{_x,_T,_x,_T, _x,_T,_x,_x, _x,_x,_x,_T, _T,_x,_x,_x, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_x,_x,_x},
		{_x,_T,_x,_T, _x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _x,_T,_x,_T, _x,_x,_x,_x, _x,_x,_x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_x,_T,_T, _T,_T,_x,_T, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_x,_T,_x, _T,_x,_x,_x, _x,_x,_x,_x},
		{_x,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_x,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_x},
		{_x,_T,_T,_T, _x,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_x,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_x},
		{_x,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _x,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_x},
		{_x,_T,_T,_T, _x,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _x,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_x},
		{_x,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_x,_T,_x},
		{_x,_T,_T,_T, _x,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _x,_x,_T,_x},
		{_x,_T,_x,_T, _x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _x,_x,_x,_x, _T,_x,_x,_x, _x,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_x, _T,_x,_x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_x,_T,_x, _T,_x,_x,_x, _x,_x,_x,_x},
		{_x,_T,_x,_T, _x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_x,_x,_x, _x,_x,_T,_T, _x,_T,_x,_T, _x,_T,_T,_x, _T,_x,_x,_x},
		{_x,_T,_x,_T, _x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_x,_x,_x, _x,_x,_T,_T, _x,_T,_x,_T, _x,_T,_x,_x, _T,_x,_x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_x,_x, _T,_x,_T,_x, _T,_x,_x,_x, _x,_x,_x,_x},
		{_x,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _x,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_x}

	};

#if PARSER_WITH_AST
	public SynTree ast_root;
	Stack ast_stack;
	
	void AstAddTerminal() {
        SynTree st = new SynTree( t );
        ((SynTree)(ast_stack.Peek())).children.Add(st);
	}

	bool AstAddNonTerminal(int kind, string nt_name, int line) {
        Token ntTok = new Token();
        ntTok.kind = kind;
        ntTok.line = line;
        ntTok.val = nt_name;
        SynTree st = new SynTree( ntTok );
        ((SynTree)(ast_stack.Peek())).children.Add(st);
        ast_stack.Push(st);
        return true;
	}

	void AstPopNonTerminal() {
        ast_stack.Pop();
	}
#endif
	
} // end Parser


public class Errors {
	public int count = 0;                                    // number of errors detected
	public System.IO.TextWriter errorStream = Console.Out;   // error messages go to this stream
	public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text

	public virtual void SynErr (int line, int col, int n) {
		string s;
		switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "ident expected"; break;
			case 2: s = "number expected"; break;
			case 3: s = "string expected"; break;
			case 4: s = "badString expected"; break;
			case 5: s = "char expected"; break;
			case 6: s = "\"COMPILER\" expected"; break;
			case 7: s = "\"IGNORECASE\" expected"; break;
			case 8: s = "\"TERMINALS\" expected"; break;
			case 9: s = "\"CHARACTERS\" expected"; break;
			case 10: s = "\"TOKENS\" expected"; break;
			case 11: s = "\"PRAGMAS\" expected"; break;
			case 12: s = "\"COMMENTS\" expected"; break;
			case 13: s = "\"FROM\" expected"; break;
			case 14: s = "\"TO\" expected"; break;
			case 15: s = "\"NESTED\" expected"; break;
			case 16: s = "\"IGNORE\" expected"; break;
			case 17: s = "\"PRODUCTIONS\" expected"; break;
			case 18: s = "\"=\" expected"; break;
			case 19: s = "\".\" expected"; break;
			case 20: s = "\"END\" expected"; break;
			case 21: s = "\"+\" expected"; break;
			case 22: s = "\"-\" expected"; break;
			case 23: s = "\"..\" expected"; break;
			case 24: s = "\"ANY\" expected"; break;
			case 25: s = "\"<\" expected"; break;
			case 26: s = "\">\" expected"; break;
			case 27: s = "\"<.\" expected"; break;
			case 28: s = "\".>\" expected"; break;
			case 29: s = "\"|\" expected"; break;
			case 30: s = "\"WEAK\" expected"; break;
			case 31: s = "\"(\" expected"; break;
			case 32: s = "\")\" expected"; break;
			case 33: s = "\"[\" expected"; break;
			case 34: s = "\"]\" expected"; break;
			case 35: s = "\"{\" expected"; break;
			case 36: s = "\"}\" expected"; break;
			case 37: s = "\"SYNC\" expected"; break;
			case 38: s = "\"IF\" expected"; break;
			case 39: s = "\"CONTEXT\" expected"; break;
			case 40: s = "\"(.\" expected"; break;
			case 41: s = "\".)\" expected"; break;
			case 42: s = "??? expected"; break;
			case 43: s = "this symbol not expected in Coco"; break;
			case 44: s = "this symbol not expected in TokenDecl"; break;
			case 45: s = "invalid TokenDecl"; break;
			case 46: s = "invalid AttrDecl"; break;
			case 47: s = "invalid SimSet"; break;
			case 48: s = "invalid Sym"; break;
			case 49: s = "invalid Term"; break;
			case 50: s = "invalid Factor"; break;
			case 51: s = "invalid Attribs"; break;
			case 52: s = "invalid TokenFactor"; break;

			default: s = "error " + n; break;
		}
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}

	public virtual void SemErr (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}

	public virtual void SemErr (string s) {
		errorStream.WriteLine(s);
		count++;
	}

	public virtual void Warning (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
	}

	public virtual void Warning(string s) {
		errorStream.WriteLine(s);
	}
} // Errors


public class FatalError: Exception {
	public FatalError(string m): base(m) {}
}
}