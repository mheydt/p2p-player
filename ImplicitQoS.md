## Metodo di classificazione dei brani (QoS implicita) ##



### Quality Of Service Implicita ###

La qualità del servizio verrà gestita sia in maniera implicita che in maniera esplicita.
La QoS esplicita esula dalla trattazione di questa pagina.<br />
Per **QoS Implicita** si intende una gestione della qualità del servizio che non richiede l'intervento diretto dell'utente. La gestione della QoS implicita si basa su un coefficiente che permetterà la classificazione dei brani sia da parte dell'utente che da parte del Sistema.

### Coefficiente di Qualità ###

Il **coefficiente di qualità** (QC) è influenzato da tre coefficienti distinti:
  * **Affinità con la Ricerca** (_AFF_): tiene conto di quanto il brano sia pertinente alla ricerca fatta dall'utente. Si tratta di una affinità sintattica e non semantica.
  * **Qualità del Peer** (_PQ_): tiene conto del carico corrente e della distanza sulla DHT del peer che deve fornire il brano.
  * **Qualità del File** (_FQ_) : tiene conto della qualità del File contenente il brano ( bit-rate del brano, codifica, ecc)

Il coefficiente di qualità si trova assegnando dei pesi ai sotto-coefficienti secondo la seguente formula:
```
QC = 0.4*PQ + 0.4*FQ + 0.2*AFF
```
Il coefficiente di affinità ha un peso minore rispetto agli altri in quanto, non considerando aspetti semantici, rappresenta un informazione sulla quale l'utente può avere maggiori capacità di discernimento.

#### Coefficiente di Affinità ####

Il coefficiente di affinità è un valore compreso tra 0 e 1 che indica l'affinità sintattica dei metadati del brano con la ricerca effettuata dall'utente. Il valore del coefficiente dipende da quale percentuale dei metadati è coperta dalla ricerca dell'utente. Ad esempio la ricerca "slayer rain" avrà un affinità con il brano _Raining Blood_ degli _Slayer_ contenuto nell'album _Reign in Blood_ sarà _0.67_.
Il calcolo viene effettuato nel seguente modo: la ricerca viene divisa in parole distinte ed ognuna di questa verrà processata separatamente. Ogni parola viene ricercata all'interno delle tre etichette del brano (Titolo, Artista, Album) ed il suo contributo sarà al valore massimo delle presenze non nulle della parola nelle varie etichette. La presenza è calcolata come il rapporto tra la lunghezza della parola processata e la lunghezza del campo esclusi gli spazi nel caso in cui la parola sia presente, altrimenti vale zero. Quindi riprendendo l'esempio precedente la presenza della parola "rain" all'interno del campo "Raining Blood" è pari a 4/12=0.33. Il contributo della parola "blood" nel brano sarà pari al massimo tra le presenze: nel titolo ha una presenza pari a 5/12, 0 nell'artista e 5/12 nell'album quindi sarà 5/12. Nel caso in cui la parola processata è contenuta nella stessa etichetta della parola precedente si aggiungerà un contributo con peso uguale agli altri e valore pari alla somma della lunghezza delle parole diviso la lunghezza dell'etichetta esclusi gli spazi. La formula finale è la seguente:
```
AFF=sum(P*max(presenze))+sum(P*contributo_concatenazioni)
```
dove P=1/num\_parole\_ricerca.<br />
Calcoliamo l'affinità del brano con la ricerca "slayer rain blood". La parola "slayer" ha
presenze 0,1,0 nelle etichette Titolo,Artista,Album quindi il suo valore è 1. La parola "rain" ha presenze 0.33,0,0 quindi il suo valore è 0,33. La parola "blood" ha presenze 0.42,0,0.42 quindi il suo valore è 0.42; inoltre la parola precedente è contenuta nell'etichetta Titolo quindi il contributo delle concatenazioni sarà pari a (4+5)/12 = 0.75. Il peso di ogni parola è pari a P=1/3=0.33. Il coefficiente di affinità sarà AFF=0.33\*1+0.33\*0.33+0.33\*0.42+0.33\*0.75=0.825. Se invece la ricerca è "slayer raining blood" il contributo della seconda parola sarà 0.58 e quello delle concatenazioni sarà 1 quindi il coefficiente di affinità sarà 1.<br />
_Dettagli Implementativi_<br />
Il calcolo di questo coefficiente si può effettuare con le funzioni associate alle stringe del linguaggio; il set di funzioni richiesto è: substring(),len(),replace(),indexof(). Di seguito lo pseudo codice dell'algoritmo di calcolo:
```
concatenazione_titolo=0
concatenazione_album=0
concatenazione_artista=0
valori=list()
for parola in ricerca :
  max=-1;
  for etichetta in etichette_sensibili:
    etichetta_no_spazi=etichetta.replace(" ","")
    if etichetta_no_spazi.indexof(parola)!=-1 :
      concatenazione_etichetta+=len(parola)
      valore_corrente=len(parola)/len(etichetta_no_spazi)
      if valore_corrente>max :
        max=valore_corrente
    else:
      valori.append(len(concatenazione_etichetta)/len(etichetta_no_spazi))
      concatenazione_etichetta=0
  valori.append(max);
peso=1/len(ricerca)
aff=0;
for valore in valori:
  aff+=(peso*valore)
```

#### Coefficiente di Qualità del File ####

Il coefficiente di Qualità del File è un valore compreso tra 0 e 1 che indica la qualità audio del file. Il coefficiente dipende da 3 parametri:
  * **Bit Rate** (_BR_): il contributo della bitrate è calcolato con una funzione in modo che 32kbps (minima bitrate definita dallo standard) risulti pari a zero, 192kbps pari a 0.78 e 320kbps(massima bitrate definita dallo standard) sia uno. Il valore del contributo della bitrate BR è pari a:
```
BR = trunc((log10(track_br/192) + 0.78) * 100 ) / 100
```
> Bitrate fuori standard genereranno valori inconsistenti; è quindi consigliato, in fase di implementazione, controllare il valore di track\_br prima di calcolare il contributo. È importante controllare che track\_br sia maggiore di zero per evitare eccezioni nel calcolo del logaritmo in base 10.<br />
> Nel caso di VBR (_Variable Bit Rate_) si utilizza come track\_br il valore medio della bitrate oppure, nel caso in cui l'encoding è stato fatto con il metodo a qualità fissa, si utilizza l'indice di qualità normalizzato tra 0 a 1. Ad esempio, gli encoder della famiglia LAME usano un indice di qualità che va da 0 a 9 quindi il contributo della bitrate sarà pari a `track_avg_br/9`.

  * **Sample Rate** (_SR_) : il contributo della Sample rate viene assegnato in base alla seguente tabella:

| **track\_sr** | **SR** |
|:--------------|:-------|
| 32000Hz       | 0.2    |
| 44100Hz       | 0.8    |
| 48000Hz       | 1.0    |

> Questi tre valori sono gli unici definiti dallo standard.
  * **Channel Mode** (_CM_): il contributo del Channel Mode viene assegnato in base alla seguente tabella:

| **track\_cm** | **CM** |
|:--------------|:-------|
| Single channel (Mono) | 0.2    |
| Dual channel (2 mono channels) | 0.5    |
| Joint stereo (Stereo) | 0.8    |
| Stereo        | 1.0    |

> Questi quattro valori sono gli unici definiti dallo standard.

Il coefficiente di qualità del file si calcola a partire dai tre contributi descritti in base alla seguente formula:
```
FQ = 0.5 * BR + 0.3 * CM + 0.2 * SR
```

_Dettagli Implementativi_<br />
I dati sul file necessari per i calcoli del coefficiente di qualità del file possono essere calcolati mediante librerie esterne. Di seguito sono elencati alcune librerie esterne compatibili con C# ed il .NET Framework che forniscono le funzionalità richieste dai calcoli:
  * **`UltraID3Lib`** ([Download](http://home.fuse.net/honnert/UltraID3Lib/))
  * **`TagLib#`** ([Download 1](http://taglib-sharp.sourcearchive.com/) | [Download 2](http://download.banshee.fm/taglib-sharp))

#### Coefficiente di Qualità del Peer ####
Il coefficiente di qualità del Peer viene calcolato a partire dalla quantità di richieste bufferizzate e non ancora servite dal peer (PR). Definendo Q il numero massimo di richieste che il buffer può contenere, il coefficiente di qualità del peer si calcola secondo la seguente formula:
```
PQ = PR/Q
```
Otteniamo anche in questo caso una quantità compresa tra 0 e 1.