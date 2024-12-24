/**

--- Day 23: LAN Party ---

As The Historians wander around a secure area at Easter Bunny HQ, you come across posters for a LAN party scheduled for today! Maybe you can find it; you connect to a nearby datalink port and download a map of the local network (your puzzle input).

The network map provides a list of every connection between two computers. For example:

kh-tc
qp-kh
de-cg
ka-co
yn-aq
qp-ub
cg-tb
vc-aq
tb-ka
wh-tc
yn-cg
kh-ub
ta-co
de-co
tc-td
tb-wq
wh-td
ta-ka
td-qp
aq-cg
wq-ub
ub-vc
de-ta
wq-aq
wq-vc
wh-yn
ka-de
kh-ta
co-tc
wh-qp
tb-vc
td-yn

Each line of text in the network map represents a single connection; the line kh-tc represents a connection between the computer named kh and the computer named tc. Connections aren't directional; tc-kh would mean exactly the same thing.

LAN parties typically involve multiplayer games, so maybe you can locate it by finding groups of connected computers. Start by looking for sets of three computers where each computer in the set is connected to the other two computers.

In this example, there are 12 such sets of three inter-connected computers:

aq,cg,yn
aq,vc,wq
co,de,ka
co,de,ta
co,ka,ta
de,ka,ta
kh,qp,ub
qp,td,wh
tb,vc,wq
tc,td,wh
td,wh,yn
ub,vc,wq

If the Chief Historian is here, and he's at the LAN party, it would be best to know that right away. You're pretty sure his computer's name starts with t, so consider only sets of three computers where at least one computer's name starts with t. That narrows the list down to 7 sets of three inter-connected computers:

co,de,ta
co,ka,ta
de,ka,ta
qp,td,wh
tb,vc,wq
tc,td,wh
td,wh,yn

Find all the sets of three inter-connected computers. How many contain at least one computer with a name that starts with t?

Part 2 :
    Voir "Problème de la clique"
    Probablement : Bron–Kerbosch algorithm (Trouver la plus grande clique dans un graphe)
    Base de la solution : https://www.reddit.com/r/adventofcode/comments/1hkgj5b/comment/m3klwug/
**/

class Program
{

    // Définir connections comme une variable globale statique
    private static Dictionary<string, HashSet<string>> connections;

    static void Main()
    {
        // Lecture des lignes du fichier
        string[] lines = File.ReadAllLines("input.txt");

        // Création du dictionnaire pour les connexions
        connections = new Dictionary<string, HashSet<string>>();

        foreach (string line in lines)
        {
            string[] p = line.Split("-");
            
            // Ajout des nœuds au dictionnaire
            if (!connections.ContainsKey(p[0]))
                connections[p[0]] = new HashSet<string>();
            if (!connections.ContainsKey(p[1]))
                connections[p[1]] = new HashSet<string>();

            // Ajout des connexions bidirectionnelles
            connections[p[0]].Add(p[1]);
            connections[p[1]].Add(p[0]);
        }

        // Résultats pour la première partie
        HashSet<string> results = new HashSet<string>();

        foreach (string x in connections.Keys)
        {
            foreach (string y in connections[x])
            {
                foreach (string z in connections[y])
                {
                    if (connections[x].Contains(z) && (x.StartsWith('t') || y.StartsWith('t') || z.StartsWith('t')))
                    {
                        List<string> t = new List<string> { x, y, z }.OrderBy(s => s).ToList();
                        results.Add(string.Join(",", t));
                    }
                }
            }
        }

        // Résultats pour la deuxième partie
        HashSet<string> cliques = new HashSet<string>();

        foreach (string node in connections.Keys)
        {
            BronKerbosch(new HashSet<string> { node }, new HashSet<string>(connections[node]), new HashSet<string>(), cliques);
        }

        int answer1 = results.Count;
        string answer2 = cliques.OrderByDescending(c => c.Length).FirstOrDefault() ?? "";

        // Affichage des résultats
        Console.WriteLine($"Part 1 answer: {answer1}");
        Console.WriteLine($"Part 2 answer: {answer2}");
    }

    /// <summary>
    /// Algorithme de Bron-Kerbosch : https://fr.wikipedia.org/wiki/Algorithme_de_Bron-Kerbosch
    /// </summary>
    /// <param name="R">Current clique</param>
    /// <param name="P">Candidates</param>
    /// <param name="X">Excluded</param>
    /// <param name="O">Output</param>
    static void BronKerbosch(HashSet<string> R, HashSet<string> P, HashSet<string> X, HashSet<string> O)
    {
        if (P.Count == 0 && X.Count == 0)
        {
            O.Add(string.Join(",", R.OrderBy(s => s))); // Ajoute la clique maximale triée à O
            return;
        }

        // Copie de P
        HashSet<string> PC = new HashSet<string>(P);

        foreach (string v in P)
        {
            // Obtenez les connexions pour le nœud courant
            HashSet<string> C = connections[v];

            // Appel récursif avec mise à jour des ensembles
            BronKerbosch(
                new HashSet<string>(R) { v }, // Ajoute v à R
                new HashSet<string>(PC.Intersect(C)), // Intersection de PC et C
                new HashSet<string>(X.Intersect(C)), // Intersection de X et C
                O
            );

            // Mise à jour de PC et X
            PC.Remove(v);
            X.Add(v);
        }
    }
}
