# 3. The 169 matrix
Here I would strongly recommend:
$$
\mathbb{M}
$$

for the 169-cell matrix as a mathematical object.

Why calligraphic \(M\)?

Because it distinguishes the abstract classification structure from a concrete numeric matrix.

For example:

$$ \mathcal M = \{m_{ij}\mid i,j\in I\} $$

where

$$ I=\{2,\ldots,14\}. $$

But there is an important distinction:

Matrix
$$ \boxed{\mathcal M} $$

means the entire 169-cell structure.

Individual cell

Use lowercase:

$$ \boxed{m} $$

or, even better when coordinates matter:

$$ \boxed{m_{ij}} $$

So:

$$ m_{ij}\in\mathcal M. $$

This gives you exactly the uppercase/lowercase relationship you were looking for:

$$ \boxed{\mathcal M \longleftrightarrow m} $$

or more precisely:

$$ \boxed{\mathcal M=\{m_{ij}\}} $$
4. But there is a subtle problem with $m_{ij}$ 

Your Cell is not really just an arbitrary matrix element. It has semantic identity determined by the two rank coordinates.

Therefore I would distinguish:

$$
m_{\rho,\lambda} 
$$

where both coordinates are ranks.

However, you already have \(\rho\) meaning rank magnitude, so using another Greek letter for the second rank is useful.

For example:

$$ \boxed{m_{\rho_1,\rho_2}} $$

with

$$ \rho_1,\rho_2\in R. $$

Then:

diagonal:
$$ m_{\rho,\rho} $$
suited:
$$ m_{\rho_l,\rho_h}^{s} $$
offsuit:
$$ m_{\rho_h,\rho_l}^{o} $$

although I would probably not put the s/o in the mathematical identity, because the coordinate ordering already encodes it.

5. Your coordinate convention maps beautifully

Given your existing Cell encoding:

$$ (x,y)\in\{2,\ldots,14\}^2 $$

we can define the matrix cell as

$$ \boxed{m_{xy}} $$

with:

$$ x,y\in I=\{2,\ldots,14\}. $$

Then:

$$ x=y \quad\Rightarrow\quad \text{pair} $$ $$ x<y \quad\Rightarrow\quad \text{suited} $$ $$ x>y \quad\Rightarrow\quad \text{offsuit}. $$

For example:

$$ m_{14,14}=AA $$ $$ m_{13,14}=AKs $$ $$ m_{14,13}=AKo. $$

This is particularly elegant because the same coordinates completely determine the semantic category.

6. One distinction I would introduce now

I would use three different levels of notation:

Card level
$$ \boxed{x=\langle\rho,\sigma\rangle} $$
Pocket level
$$ \boxed{p=\{x_1,x_2\}} $$
Cell level
$$ \boxed{m_{ij}} $$

and the whole classification:

$$ \boxed{\mathcal M} $$

So the fundamental hierarchy becomes:

$$ \boxed{ R,S \rightarrow C \rightarrow P \rightarrow \mathcal M } $$

with:

$$ \rho\in R,\quad \sigma\in S,\quad x\in C,\quad p\in P,\quad m\in\mathcal M. $$

But importantly, \(m\) is not a pocket. It is a category that defines a subset of pockets.

That gives us the next very natural definition:

$$ \boxed{ \Phi:\mathcal M\rightarrow\mathcal P(P) } $$

where

$$ \Phi(m) $$

is the set of concrete pockets represented by cell \(m\).

For example:

$$ \Phi(m_{13,14}) = \{ A_sK_s,A_hK_h,A_dK_d,A_cK_c \}. $$

Thus:

$$ |\Phi(m)|\in\{6,4,12\}. $$

This is, in my opinion, the cleanest mathematical foundation for the Cell implementation we were discussing.

Final notation I would settle on
$$ \boxed{ \begin{aligned} R &= \text{ranks}\\ S &= \text{suits}\\ C &= R\times S && \text{cards}\\ x &= \langle\rho,\sigma\rangle && \text{a card}\\ P &= \binom C2 && \text{pockets}\\ p &= \{x_1,x_2\} && \text{a pocket}\\ \mathcal M &= \text{169-cell matrix}\\ m_{ij} &\in\mathcal M && \text{a cell}\\ \Phi(m_{ij}) &\subseteq P && \text{pockets represented by the cell} \end{aligned} } $$

I would not use \(C\) for an individual card. Your instinct to reserve \(C\) for the set of cards is correct; \(x\) is cleaner for the individual card.


such that:

$$ D_m(p)= \begin{cases} 1 & p\in\Phi(m)\\ 0 & p\notin\Phi(m) \end{cases} $$