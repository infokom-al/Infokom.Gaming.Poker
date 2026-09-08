## Equity: Range of Cells
If $\mathcal{M}$ is the equity matrix:

$$
\mathcal{M} = 
\begin{bmatrix}
	m_{0,0}	&	m_{0,1}	&	...	&   m_{0,n}    \\
	m_{1,0}	&	m_{1,1}	&	...	&   m_{0,n}    \\
	...		&	...		&	...	&   ...         \\
	m_{n,0}	&	m_{n,1}	&	...	&   m_{n,n}    \\

\end{bmatrix}
$$

with $n = 13$,  composed by the elements in the generic form decribed as $m_{i,j} \in \mathcal{M}$, to indicate the cell at row $i$ and column $j$, then a range $\overline{m}$ is a collection of cells. By the name, we deduce a range is a contigous region of $\mathcal{M}$, anyway we certainly saffirm that a range is a subset of the elements of the range matrix:
$$
\hat{m} \subseteq \mathcal{M}
$$ 

so, a range is a collection of cells, and a cell is a collection o pockets. With some abuse, for a given range we can talk about pocket hands in that range, implicitly meaning all the pocket hands of the cells contained in that range.

$$
\Phi(\hat{m}) = \bigcup_{m \in \mathcal{\hat{m}}} \Phi(m)
$$

### Range math
Given
$$\mathcal{M} = \{m | \text{m is one of the 169 valid cells}\}$$
if $\mathcal{R} \subseteq \mathcal{M}$ is a range then the following properties apply:
$$\mathcal{R} \cup \mathcal{R} = \mathcal{R}$$
$$\mathcal{R} \cap \mathcal{R} = \mathcal{R}$$
$$\mathcal{R} \oplus \mathcal{R} = \varnothing$$
$$\mathcal{R} \cup \varnothing = \mathcal{R}$$
$$\mathcal{R} \cap \varnothing = \varnothing$$
$$\mathcal{R} \oplus \varnothing = \varnothing$$
$$\overline{(\overline{\mathcal{R}})} = \mathcal{R}$$

### Range weight
Each cell $m \in \mathcal{M}$ has a different number of concrete pockets (pair of cards). We can define the weight of a cell $m$ as:
$$w(m) = \begin{cases} 
	6 	& \text{ if } M \text{ is paired}\\
	4 	& \text{ if } M \text{ is suited}\\
	12 	& \text{ if } M \text{ is offsuit}\\
\end{cases}$$
Similarly, if $\mathcal{R} \subseteq \mathcal{M}$ is a range, the weight of that range is defined as:
$$W(\mathcal{R})=\sum_{m\in \mathcal{R}} {w(m)}$$
also named the combinatorial size of that range


$$
C_{n,k}=\binom{n}{k} = \prod_{i=1}^{k} C_{n,k}{[i]}
$$

$$
C_{n,k}{[i]} = \frac{n+1-i}{i}=\frac{n+1}{i}-1
$$