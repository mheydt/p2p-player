Si farà ESPLICITO riferimento all'articolo: http://xlattice.sourceforge.net/components/protocol/kademlia/specs.html.
L'implementazione rispecchia l'articolo stesso.

Ciascun brano è identificato da:
  * un ID3 Tag secondo specifiche da determinare
  * hash dell'ID3 Tag (sha1)
  * hash del file (sha1)

La FIND\_VALUE di Kademlia verrà modificata come segue:
Inviamo sulla rete l'ID3 Tag e il suo hash. Per muoverci fra i nodi secondo il processo definito nell'articolo come "Iterative Find Value" usiamo l'hash dell'id3. Per controllare se il nodo contiene il brano effettuiamo un matching basato sul plain ID3 Tag in base alle metriche stabilite nel Wiki ImplicitQoS. Il metodo ritornerà l'hash (o una lista di hash) dei file aventi ID3 Tag sufficientemente affini.


---


# Revisione in data 24/04/2011 #

Ricapitolo,

abbiamo due differenti collezioni in schema "chiave -> valore".
La prima collezione (primary collection) è del tipo: "keyword -> TAG\_ID"; la seconda collezione (secundary collection) è del tipo "TAG\_ID -> (TAG, list(URL))".

FASE DI STORAGGIO DATI:
l'utente fornisce i tre campi:
  * titolo
  * autore
  * album
ed inoltre supponiamo al file sia associato un TAG.

Supponiamo di avere la seguente tripla:
  * titolo: Strawberry Fields Forever
  * autore: Beatles
  * album: Glass Onions
supponiamo sia fornito il TAG.

Inserisco in secundary collection una coppia del tipo "TAG\_ID -> TAG, list(URL)" dove il TAG\_ID è un UUID creato da RAVEN, la lista di url rappresenta una lista di tutti coloro i quali hanno fatto lo store dello stesso TAG.
Inserisco in primary collection 6 coppie "keyword -> TAG\_ID" quindi:

  1. "Strawberry -> list(TAG\_ID,...)"
  1. "Fields -> list(TAG\_ID,...)"
  1. "Forever -> list(TAG\_ID,...)"
  1. "Beatles -> list(TAG\_ID,...)"
  1. "Glass -> list(TAG\_ID,...)"
  1. "Onions -> list(TAG\_ID,...)"

si indicano liste perché ciascuna parola può riferire più tag. Ad esempio "Onions" identifica questa canzone dei Beatles ma anche tutte quelle degli Oliver Onions.

FASE DI RICERCA:
Su Kademlia faccio muovere la frase scritta dall'utente.
La persistenza splitta la frase nelle parole che la compongono e ricerca i TAG\_ID associati ad ognuna di esse (se ve ne sono). La persistenza restituisce agli strati superiori tutti i TAG così ottenuti.

Il sistema è questo. Perfezionamenti consistono in:
  1. Scartare sia in fase di storaggio che in fase di ricerca le parole semanticamente inutili (articoli, congiunzioni, ecc....)
  1. Attuare un metodo a campana di gauss sui risultati; ossia: supponiamo che un dato TAG sia stato riscontrato durante la ricerca N volte ed N sia il massimo, verranno presi tutti i risultati che si trovino nell'intorno di questo massimo. Ossia quelli che sono apparsi minimo (N-e) volte.
In merito all'ultimo punto, supponiamo che l'utente abbia ricercato: "Strawberry Fields Forever Beatles".
Il tag storato nell'esempio precedente apparirà 4 volte nei risultati perché riferito da tutte le parole; tuttavia, anche vari brani degli Strawberry Fields appariranno nella ricerca 2 volte. Si deve predisporre un metodo a campana di gauss che sia in grado di eliminare parte di questi risultati.
Altra soluzione può essere quella di sfruttare il TAG per eliminare tutti quei risultati che non contengono nel tag almeno una delle parole  non riferite dalla ricerca. Ad esempio: tutti i brani degli Strawberry Fields non conterranno la parola Beatles e quindi potranno essere scartati. D'altronde tutti i brani dei beatles verrebbero trovati dalla semplice ricerca per keyword ma eliminati dei metodi a campana e da questo che incrocia i tag. Infatti tutti i brani dei beatles sarebbero riferiti al più 1 volta dalla ricerca ed inoltre nessuno di essi contiene nel tag le parole Strawberry, Fields e Forever.