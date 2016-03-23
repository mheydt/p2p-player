# Introduzione #

Ho deciso di ispirarmi fortemente al progetto: GoalBit.
Sebbene questo sia basato sul BitTorrent e quindi un sistema a Tacker ritengo sia possibile riadattare il sistema di frammentazione al nostro caso.
Mi limiterò ad inserire qui i dettagli man mano che vado avanti con la progettazione.


# Definizioni #

I peer nel nostro caso non sono distinguibili in base a un ruolo specifico. Ogni peer si occupa, pertanto sia dell'instradamento che del servizio vero e proprio. Non è, quindi, in alcun modo riproponibile un sistema in cui alcuni peer siano considerabili come Tracker o Superpeer o, comunque, elevarsi al di sopra degli altri.

Al fine di permettere una diffusione ottimizzata del flusso sarà necessario un partizionamento dello stesso in componenti atomiche che chiameremo **chunk**.

Un chunk avrà dimensioni fisse pari a **60Kb** e ad esso sarà associato un identificativo univoco (per il flusso); tali ID possono essere assegnati in maniera incrementale in un numero compreso nell'intervallo **`[0:(len(flusso)/len(chunk))]`**. A questo identificativo diamo il nome di **CID**.

Al peer richiedente il flusso viene associata una **Sliding Window** che rappresenta l'area di interesse per il peer sul flusso. La Window è definita da un estremo inferiore detto **Base** e comprende la parte del flusso fra il punto identificato da **Base** e il termine del flusso stesso.

Ultima importante caratteristica di un flusso è il cosiddetto **Active Buffer**. Chiamiamo Active Buffer un sottoinsieme della Sliding Window che si estende dalla base comprendendo la parte di flusso riproducibile senza interruzioni. L'active buffer può essere identificato mediante un identificativo detto **ABI** che altro non è che il CID del chunk immediatamente successivo alla parte riproducibile.

Molto può essere spiegato dall'immagine:

![http://sourceforge.net/apps/mediawiki/goalbit/nfs/project/g/go/goalbit/thumb/0/0f/ABI.png/350px-ABI.png](http://sourceforge.net/apps/mediawiki/goalbit/nfs/project/g/go/goalbit/thumb/0/0f/ABI.png/350px-ABI.png)

# Funzionamento #

Il funzionamento del sistema è essenzialmente basato su tre assunti:
  1. Ogni brano condiviso è perfettamente completo (non è quindi possibile distinguere i chunk all'interno dell stesso brano in più rari o meno); tutti i chunk sono ugualmente rari.
  1. Ogni risorsa (brano) è identificato univocamente sulla rete mediante un hash SHA1 di 160 bit.
  1. Si possiede una lista di tutti i peer a cui richiedere i chunk ottenuta mediante protocollo di ricerca ordinati in base alla PQ.

Il metodo usato per la richiesta dei chunk prende il nome di **Chunk Overlapping**.

## Messaggi scambiati ##
I due messaggi scambiati fondamentali sono la **CHKRQ** (Chunk request) e la **CHKRS** (Chunk response).

### Chunk Request ###
| **Field** | **Details** |
|:----------|:------------|
| Message   | CHKRQ       |
| RID       | Resource Identifier. Rappresenta univocamente la risorsa. E` effettivamente l'hash in sha1 (160bit) della risorsa stessa.|
| CID       | Chunk Identifier. Numero progressivo che identifica il chunk nel brano. |
| Active Buffer | Dimensione dell'active buffer del richiedente |

### Chunk Response ###
| **Field** | **Details** |
|:----------|:------------|
| Message   | CHKRS       |
| RID       | Resource Identifier. Rappresenta univocamente la risorsa. E` effettivamente l'hash in sha1 (160bit) della risorsa stessa.|
| CID       | Chunk Identifier. Numero progressivo che identifica il chunk nel brano. |
| Serving Buffer | Dimensione del serving buffer del fornitore |
| Payload   | Il chunk vero e proprio; 60kb ordinario, l'ultimo probabilmente sarà più piccolo |

## Peer Richiedente ##
Supponendo di trovarsi in un dato istante T il nostro peer, come nella precedente immagine, avrà una certa ABI.

Il funzionamento si basa sul succedersi di due processi che lavorano sulla lista dei peer.

Un processo (consumatore) iterativamente preleva il primo peer dalla lista (quello con PQ maggiore) e a questo viene richiesto il chunk identificato dall'indice ABI. In questo modo il processo tenta di consumare interamente la lista.

Il secondo processo (produttore) ricevuto il chunk. Riposiziona il peer nella posizione corretta all'interno della lista (rivalutandone la PQ).

Al fine di impedire la mancanza imprevista di chunk; qualora un chunk richiesto successivamente arrivi prima di uno richiesto prima (questo significa che un peer con più alta PQ teorica ha ritardato rispetto ad un altro con PQ più bassa). Il peer che non ha ancora consegnato il chunk e l'altro scambiano le proprie posizioni in lista e il chunk non pervenuto viene richiesto al successivo peer in lista.

Chunk ottenuti più volte vengono semplicemente scartati.

## Peer Fornitore ##
Il peer fornitore bufferizza (all'interno del **Serving Buffer**) tutte le richieste ricevute e sceglie dal buffer di volta in volta secondo un metodo che si potrebbe definire **MNF** (More Needed First). Si sceglie dal buffer la richiesta avente Active Buffer di minori dimensioni.

# Collegamenti Utili #
Alcuni link utili in riferimento all'argomento trattato:

  * GoalBit: http://sourceforge.net/apps/mediawiki/goalbit/index.php?title=Goalbit_architecture