# Solution pour la partie 2 de l'exercice 14

En gros, c'est lorsque vous avez plusieurs séquences périodiques et que vous voulez trouver le moment où elles s'alignent.

\[
s_1 = o_1 + i \cdot p_1
\]
\[
s_2 = o_2 + i \cdot p_2
\]
\[
...
\]
\[
s_N = o_N + i \cdot p_N
\]

(pour tout entier \( i \geq 0 \)).

Si vous avez la période de chaque séquence (\( p_1, p_2, ... \)) et leur phase (\( o_1, o_2, ... \)), vous pouvez trouver le premier nombre qui appartient à toutes les séquences.

Dans ce problème, il y avait deux séquences :

1. \( s_1 \) : Les moments où le motif horizontal apparaissait, et  
2. \( s_2 \) : Les moments où le motif vertical apparaissait.

Pour moi, le motif horizontal apparaissait pour la première fois à \( t = 50 \), puis tous les 103 images après cela. Le motif vertical apparaissait pour la première fois à \( t = 85 \), puis tous les 101 images après cela.

Ainsi, les entrées pour le théorème des restes chinois étaient :

\[
s_1 = 50 + i \cdot 103
\]
\[
s_2 = 85 + i \cdot 101
\]

Vous parcourez ces séquences de cette façon :

\[
i = 0 \quad s_1[i] = 50 \quad s_2[i] = 85
\]
\[
i = 1 \quad s_1[i] = 153 \quad s_2[i] = 186
\]
\[
...
\]

Vous voyez donc que cela donne les images où les motifs apparaissent. On essaie de trouver le moment où les deux motifs apparaissent simultanément, c'est-à-dire la première fois où \( s_1[i] = s_2[j] = n \). (Remarquez que c'est \( n \) que nous recherchons ; les indices \( i \) et \( j \) ne nous importent pas.)

On peut utilisé un [solveur en ligne](https://www.dcode.fr/chinese-remainder). Vous devez faire attention avec les solveurs en ligne, car certains sont simplement cassés, mais celui-ci fonctionne. Les restes seraient 50 et 85, et les modulos seraient 103 et 101. Le solveur donne 7054, qui est la réponse.