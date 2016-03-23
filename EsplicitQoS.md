## Metodo di classificazione dei brani (QoS esplicita) ##

_Pagina in Costruzione_



### Quality Of Service esplicita ###

Per **QoS Esplicita** si intende la qualità che ogni utente assegna a una risorsa audio/video, dopo averla utilizzata, tramite una valutazione diretta o indiretta. Tale valutazione non influisce in modo predominante e automatico sul sistema complessivo, ma può essere di supporto agli utenti nella procedura di scelta delle risorse e, tramite un processo di selezione naturale, migliora infine la qualità del materiale presente all'interno della rete.

### Metodo di valutazione ###

La valutazione di una risorsa avviene, come già detto, dopo il suo utilizzo. Ogni utente possiede una lista dei file che ha usato e associa ad ognuno una valutazione. La valutazione può essere diretta o indiretta. Cerchiamo di capire cosa vuol dire e il perché di tale scelta.

Consideriamo il caso reale nel caso di una playlist musicale (le stesse considerazioni possono essere fatte nel caso di risorse video): un utente carica sul proprio player una playlist e lancia l'esecuzione della stessa. Generalmente dopo tale azione, l'utente medio passa ad altra attività continuando l'esecuzione del player in background, riprendendola in primo piano solo per navigare all'interno della playlist o qualora il brano scelto non corrisponda, in sostanza o in qualità, a quello che ci si aspettava all'atto della selezione. In queste condizioni appare evidente che richiedere la valutazione al temine di ogni brano, al pari di un feedback rilasciato al termine di una transazione nei diversi siti di e-commerce, non è una soluzione efficiente e soddisfacente. D'altra parte nemmeno l'idea di richiedere la valutazione complessiva al temine della playlist sembra praticabile perché, o si danneggia la reputazione dei file "buoni", a causa dei troppi file "cattivi" all'interno della playlist, o viceversa si trascura una risorsa scadente a causa di una esperienza valutata complessivamente buona. Un'altra idea potrebbe essere quella di richiedere  il feedback dell'utente in modo semi-random per creare delle statistiche sulle esperienze effettuate. Questo purtroppo non ci darebbe informazioni complete e in più il lavoro svolto da utenti responsabili verrebbe vanificato da utenti che non si interessano al rilascio di feedback.

A questo punto viene formulata la seguente soluzione: ad brano ascoltato interamente viene assegnato, indirettamente e quindi in modo trasparente all'utente, un voto positivo in quanto si può supporre, dal momento che la risorsa è stata consumata interamente, che essa corrisponda a quella desiderata. Se invece l'utente interrompe il brano durante la sua esecuzione viene richiesto esplicitamente un feedback diretto. Ci troviamo infatti nella situazione in cui l'utente ha perso il suo interesse ad ascoltare il brano oppure il brano non corrisponde alle sue aspettative. In questo caso viene registrata la valutazione negativa (la risorsa non corrisponde alle aspettative) o neutra (l'utente non è più interessato a consumare la risorsa e nulla si può dire sulla sua qualità).

In ogni caso la valutazione viene registrata solo una volta nella lista delle risorse utilizzate, anche se tale lista deve essere periodicamente aggiornata per permettere a nuove risorse di emergere all'interno della rete e sostituire le più obsolete, qualora le loro caratteristiche siano qualitativamente migliori. La lista deve contenere almeno i seguenti campi per ogni risorsa:

> - Id della risorsa <br>
<blockquote>- Valutazione assegnata alla risorsa <br>
- Data della valutazione <br>
- Scadenza della valutazione <br></blockquote>

Alla scadenza della valutazione la risorsa viene cancellata dalla lista delle risorse utilizzate.<br>
<br>
<h3>Ottenere la qualità di una risorsa</h3>

La qualità di una risorsa si ottiene allo stesso modo in cui si ottiene la risorsa stessa. Quando infatti viene effettuata la ricerca di un file all'interno della rete, viene lanciata, in modo trasparente all'utente, la ricerca degli utenti che hanno utilizzato lo stesso file e si recupera la valutazione che gli utenti gli hanno assegnato. La valutazione complessiva è pari alla somma di tutte le valutazioni recuperate nella ricerca.<br>
<br>
<img src='http://www.citilife.it/p2p.jpg' />

Seguendo il metodo di ricerca di kademlia, che interroga il nodo e restituisce il valore (se presente) o una lista dei nodi vicini (se il valore è assente), la valutazione può essere inviata come attributo della lista restituita. E' importante sottolineare infatti che un nodo non può restituire sia il brano (valore) che la valutazione, per cui la soluzione proposta sembra abbastanza soddisfacente.