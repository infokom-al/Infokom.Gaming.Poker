A deck is a tuple:

$$
\Delta = \langle R,S \rangle
$$

where
$$
R =\{\mathtt{2,3,4,5,6,7,8,9,T,J,Q,K,A}\}
$$
is the set of rank constants,
$$S = \{\mathtt{s,d,c,h}\}$$
The set
$$
\Kappa = R \times S = \{\varkappa = \langle \varrho,\varsigma \rangle \mid \varrho \in R, \varsigma \in S\}
$$
is called the set of cards and for each element $\varkappa \in \Kappa$. If $\varkappa = \langle \varrho,\varsigma \rangle$
- $\rho(\varkappa)  = \varrho $
- $\sigma(\varkappa) = \varsigma$
- $\kappa_{\varsigma}(\varrho) = \varkappa$
- $\kappa_{\varrho}(\varsigma) = \varkappa$

Bashkesia e të gjitha kombinimeve të mundshme të 2-letrave $\mathcal{C}_2$  nga një pako standarde me letra $C$ mund të përfaqësohet si bashkësia e të gjitha nënbashkësive të kartave që përmbajnë saktësisht dy letra:
$$
\mathcal{C}_2 = \binom{C}{2} = \{X\in \mathcal{P}(C): |X|=2\}
$$
 dhe madhësia e saj është:
$$
|C_2| = \binom{52}{2}=1326
$$

Given $\rho \in R$, the set 
- $\kappa_{\rho} = \{(\varrho, \varsigma\}):\varrho=\rho, \varsigma \in S\}$ is the set of all cards $\varkappa$ such that $\rho(\varkappa) = \varrho$.
- $\kappa_{\sigma} = \{(\varrho, \varsigma\}):\varrho\in R, \varsigma=\sigma\}$ is the set of all cards $\varkappa$ such that $\sigma(\varkappa) = \varsigma$.
- $\kappa_{\mathtt{A}} = \{(\mathtt{A}, \varsigma\}): \varsigma \in S\} = \{(\mathtt{A}, \mathtt{s}), (\mathtt{A}, \mathtt{d}), (\mathtt{A}, \mathtt{c}), (\mathtt{A}, \mathtt{h})\}$



Çfarë bën matrica 13×13?

Matrica nuk është vetë $C_2$; ajo është një partition / segmentation e $C_2$ sipas rank-eve dhe marrëdhënies së suit-it:
$$ 
C_2 = \bigcup_{\text{Cell } c} C_2(c) 
$$
ku çdo Cell përcakton një nënbashkësi të $C_2$. Per shembull, qeliza AKs përmban të gjitha kombinimet e mundshme të dy kartave që përfshijnë një Ace dhe një King nga suite të ndryshme. Në këtë rast:
$$
C_2^{AKs} = \{(\rho_1,\sigma),(\rho_2,\sigma) \mid \rho_1 = A, \rho_2 = K, s \in S\} 
$$

$$
|C_2^{AKs}| = 4
$$

Per $AKo$ kemi:
$$
C_2^{AKo} = \{(\rho_1,\sigma_1),(\rho_2,\sigma_2) \mid \rho_1 = A, \rho_2 = K, s_1 \neq s_2\}
$$
dhe per $C_{AA}$ kemi:
$$
C_{AA} = \{(\rho,\sigma_1),(\rho,\sigma_2) \mid \rho = A, s_1 \neq s_2\}
$$

$$
|C_{AA}| = \binom{4}{2} = 6
$$

Per nje cell $\gamma = \langle\rho_1,\rho_2,\sigma\rangle$ marre nga matrica $\Gamma[13\times 13]$ kemi keto parametra:

$$
hi(\gamma)=\max(\rho_1,\rho_2) 
$$

$$
{lo}(\gamma)=\min(\rho_1,\rho_2)
$$

$$
pair(\gamma) \iff hi(\gamma)=lo(\gamma)
$$

$$
suited(\gamma) \iff \forall p=\langle(\rho_1,\sigma_1),(\rho_2,\sigma_2)\rangle \in \gamma, \sigma_1 = \sigma_2
$$

ose ne terma koordinatash:

$$
\gamma_{x<y}
$$

Në nivelin e Cell, kjo mund të shprehet edhe thjesht nga pozicioni:

$$ \boxed{\operatorname{IsSuited}(Cell)\iff x<y} $$
Cardinality
$$ \boxed{|\operatorname{Cell}|= \begin{cases} \binom{4}{2}=6 & \text{nëse Paired}\\[4pt] 4 & \text{nëse Suited}\\[4pt] 4\cdot3=12 & \text{nëse Offsuit} \end{cases}} $$

Pra, përmbledhur:

$$ \boxed{ Cell=(Hi,Lo,\text{relation}) } $$

ku relation ∈ {Paired, Suited, Offsuit}, dhe cardinality është një funksion vetëm i kësaj relation:

$$ \boxed{ |\operatorname{Cell}|= \begin{cases} 6 & P\\ 4 & S\\ 12 & O \end{cases}} $$

Dhe në total:

$$ 13(6)+78(4)+78(12)=1326=|C_2| $$

që tregon se 169 Cell-et formojnë një partition të plotë të \(C_2\).


# 1. Primitive sets

Rank set:
$$ 
R = \{\rho_2,\rho_3,\ldots,\rho_A\} 
$$

Suit set:
$$
S = \{\sigma_s,\sigma_d,\sigma_c,\sigma_h\}
$$

Set of cards:
$$
C = R\times S 
$$
and an individual card is
$$
x=\langle \rho,\sigma\rangle\in C.
$$
where $\rho\in R$ — rank component, $\sigma\in S$ — suit component and $x \in C$ — card
# 2. A pocket
Since a pocket is an unordered pair of distinct cards:
$$
P = \binom{C}{2} 
$$
and an individual pocket can be written:
$$
p=\{x_1,x_2\}\in P
$$
This gives us a very clean hierarchy at set(type) level:
$$ 
R,\ S \quad \longrightarrow \quad C=R\times S \quad\longrightarrow \quad P=\binom{C}{2} 
$$
and at the element(instance) level we have:
$$ 
\rho,\sigma \quad \longrightarrow \quad x=\langle\rho, \sigma\rangle \quad\longrightarrow \quad p=\langle x_1,  x_2 \rangle=\langle \langle \rho_1,  \sigma_1 \rangle,  \langle \rho_2,  \sigma_2 \rangle \rangle
$$
And:
$$
|C|=52,\qquad |P|=\binom{52}{2}=1326. 
$$