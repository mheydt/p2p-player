/*****************************************************************************************
 *  p2p-player
 *  An audio player developed in C# based on a shared base to obtain the music from.
 * 
 *  Copyright (C) 2010-2011 Dario Mazza, Sebastiano Merlino
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as
 *  published by the Free Software Foundation, either version 3 of the
 *  License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Affero General Public License for more details.
 *
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *  
 *  Dario Mazza (dariomzz@gmail.com)
 *  Sebastiano Merlino (etr@pensieroartificiale.com)
 *  Full Source and Documentation available on Google Code Project "p2p-player", 
 *  see <http://code.google.com/p/p2p-player/>
 *
 ******************************************************************************************/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Persistence
{
    /// <summary>
    /// Classe astratta rappresentante un generico Repository.
    /// Questa classe viene usata per schermare le diverse implementazioni del repository attraverso un pattern Factory.
    /// Per istanziare una classe derivata da Repository usare il metodo statico GetRepositoryInstance della classe 
    /// <see cref="Persistence.RepositoryFactory"/>.
    /// </summary>
    /// 
    public abstract class Repository: IDisposable  {
        /// <summary>
        /// Metodo che si occupa del salvataggio delle informazioni all'interno del repository.
        /// Se l'elemento non esiste viene inserito, altrimenti viene soltanto aggiornato.
        /// </summary>
        /// <param name="data">Dato da salvare nel repository</param>
        /// <returns>Il risultato dell'operazione sul repository. Ritorna un valore negativo in caso di errori, altrimenti un valore
        /// che identifica l'operazione eseguita.</returns>
        /// <seealso cref="Persistence.RepositoryResponse"/>
        public abstract RepositoryResponse Save(ILoadable data);
        /// <summary>
        /// Metodo che si occupa di eliminare un elemento dal repository.
        /// </summary>
        /// <param name="id">Chiave identificativa dell'elemento che si vuole eliminare</param>
        /// <returns>Il risultato dell'operazione sul repository. Ritorna un valore negativo in caso di errori, altrimenti un valore
        /// che identifica l'operazione eseguita.</returns>
        /// <typeparam name="DBType">Tipo di dato usato nel database</typeparam>
        public abstract RepositoryResponse Delete<DBType>(string id) where DBType : IDocumentType;
        /// <summary>
        /// Metodo che si occupa di eliminare un gruppo di elementi dal repository
        /// </summary>
        /// <typeparam name="DBType">Tipo di dato usato nel database</typeparam>
        /// <param name="ids">Chiavi identificative degli elementi da eliminare</param>
        /// <returns></returns>
        public abstract RepositoryResponse BulkDelete<DBType>(string[] ids) where DBType : IDocumentType;
        /// <summary>
        /// Metodo che ritorna la dimensione del repository.
        /// </summary>
        /// <returns>La dimensione del repository.</returns>
        /// <typeparam name="DBType">Tipo di dato usato nel database</typeparam>
        public abstract int Count<DBType>() where DBType : IDocumentType;
        /// <summary>
        /// Metodo che si occupa di ritornare un elemento del repository identificato da una chiave. Il risultato viene scritto
        /// nel parametro <c>elem</c> passato in ingresso
        /// </summary>
        /// <param name="id">Chiave identificativa dell'elemento che si vuole ricercare</param>
        /// <param name="elem">Parametro di riferimento in uscita che verrà valorizzato con i dati provenienti dal repository</param>
        /// <returns>Il risultato dell'operazione sul repository. Ritorna un valore negativo in caso di errori, altrimenti un valore
        /// che identifica l'operazione eseguita.</returns>
        /// <typeparam name="DBType">Tipo di dato usato nel database</typeparam>
        public abstract RepositoryResponse GetByKey<DBType>(string id, ILoadable elem) where DBType : IDocumentType;
        /// <summary>
        /// Metodo che si occupa di ritornare gli elementi del repository che soddisfano una determinata condizione.
        /// </summary>
        /// <typeparam name="DBType">Tipo di Dato usato nel database</typeparam>
        /// <param name="cond">Condizione che gli elementi devono soddisfare</param>
        /// <param name="elems">Lista a cui verranno aggiunti gli elementi trovari</param>
        /// <returns>Il risultato dell'operazione sul repository. Ritorna un valore negativo in caso di errori, altrimenti un valore
        /// che identifica l'operazione eseguita</returns>
        public abstract RepositoryResponse GetAllByCondition<DBType>(Func<DBType, bool> cond,List<DBType> elems) where DBType: IDocumentType;
        /// <summary>
        /// Metodo che si occupa di ritornare gli elementid del repository identificati dalle chiavi fornite.
        /// </summary>
        /// <typeparam name="DBType">Tipo di dato usato nel database</typeparam>
        /// <param name="ids">Array contenente le chiavi identificative degli elementi che si voglioni ricercare</param>
        /// <param name="elems">Parametro di riferimento in uscita che verrà valorizzato con i dati proveniente dal repository</param>
        /// <returns>Il risultato dell'operazione sul repository. Ritorna un valore negativo in caso di errori, altrimenti un valore
        /// che identifica l'operazione eseguita</returns>
        public abstract RepositoryResponse GetByKeySet<DBType>(string[] ids, List<DBType> elems) where DBType: IDocumentType;
        /// <summary>
        /// Metodo che si occupa di ritornare tutti gli elementi del repository inserendoli in una collezione passata come parametro.
        /// </summary>
        /// <param name="cont">Collezione di ILoadable che verrà riempita con gli elementi estratti dal Repository</param>
        /// <returns>Il risultato dell'operazione sul repository. Ritorna un valore negativo in caso di errori, altrimenti un valore
        /// che identifica l'operazione eseguita.</returns>
        /// <typeparam name="DBType">Tipo di dato usato nel database</typeparam>
        public abstract RepositoryResponse GetAll<DBType>(ICollection<DBType> cont) where DBType : IDocumentType;
        /// <summary>
        /// Metodo per creare un indice sul repository
        /// </summary>
        /// <param name="indexName">Nome dell'indice da creare</param>
        /// <param name="indexQuery">Query che costituisce L'indice/param>
        /// <returns>Il risultato dell'operazione sul repository. Ritorna un valore negativo in caso di errori, altrimenti un valore
        /// che identifica l'operazione eseguita</returns>
        public abstract RepositoryResponse CreateIndex(string indexName, string indexQuery);
        /// <summary>
        /// Metodo per interrogare un indice
        /// </summary>
        /// <typeparam name="DBType">Tipo di dato usato nel repository</typeparam>
        /// <param name="indexName">Nome dell'indice da interrogare</param>
        /// <param name="query">Query</param>
        /// <param name="elems">Elementi dell'indice che soddisfano la condizione</param>
        /// <returns></returns>
        public abstract RepositoryResponse QueryOverIndex<DBType>(string indexName, string query, List<DBType> elems) where DBType:IDocumentType;
        /// <summary>
        /// Metodo che si occupa di aggiungere un elemento ad una proprietà di tipo array contenuta in un documento.
        /// Questo metodo evita il processo di update dell'intero documento.
        /// </summary>
        /// <param name="key">Chiave del documento da modificare</param>
        /// <param name="property">Proprietà da modificare</param>
        /// <param name="element">Elemento da aggiungere all'array</param>
        /// <returns>Il risultato dell'operazione sul repository. Ritorna un valore negativo in caso di errori, altrimenti un valore
        /// che identifica l'operazione eseguita</returns>
        public abstract RepositoryResponse ArrayAddElement(string key, string property, object element);
        /// <summary>
        /// Metodo che si occupa di rimuovere un elemento da una proprietà di tipo array contenuta in un documento.
        /// Questo metodo evita il processo di update dell'intero documento.
        /// </summary>
        /// <param name="key">Chiave del documento da modificare</param>
        /// <param name="property">Proprietà da modificare</param>
        /// <param name="index">Indice dell'elemento da rimuovere</param>
        /// <returns>Il risultato dell'operazione sul repository. Ritorna un valore negativo in caso di errori, altrimenti un valore
        /// che identifica l'operazione eseguita</returns>
        public abstract RepositoryResponse ArrayRemoveElement(string key, string property, object value);
        /// <summary>
        /// Metodo che si occupa di rimuovere degli elementi da una proprietà di tipo array contenuta in un documento.
        /// Questo metodo evita il processo di update dell'intero documento e permette di utilizzare le posizioni all'interno dell'array
        /// per identificare gli elementi da modificare
        /// </summary>
        /// <param name="key">Chiave del documento da modificare</param>
        /// <param name="property">Proprietà damodificare</param>
        /// <param name="positions">Indici degli delementi da rimuovere</param>
        /// <returns></returns>
        public abstract RepositoryResponse ArrayRemoveByPosition(string key,string property,params object[] values);
        /// <summary>
        /// Metodo che si occupa di impostare il valore di una proprietà all'interno di un documento. 
        /// Questo metodo evita il processo di update dell'intero documento.
        /// </summary>
        /// <param name="key">Chiave del documento da modificare</param>
        /// <param name="property">Proprietà da modificare</param>
        /// <param name="newValue">Nuovo valore della proprietà</param>
        /// <returns>Il risultato dell'operazione sul repository. Ritorna un valore negativo in caso di errori, altrimenti un valore
        /// che identifica l'operazione eseguita</returns>
        public abstract RepositoryResponse SetPropertyValue(string key, string property, object newValue);
        /// <summary>
        /// Metodo che si occupa di impostare il valore all'interno di un array contenuto in una proprietà
        /// Questo metodo evita l'eliminazione ed il reinserimento di un elemento nell'array
        /// </summary>
        /// <param name="key">Chiave del documento da modifica</param>
        /// <param name="property">Proprietà da modificare (deve essere un array)</param>
        /// <param name="index">Indice dell'elemento da modificare</param>
        /// <param name="obj_prop">Nome della proprietà da modificare dell'oggetto contenuto nell'array</param>
        /// <param name="value">Valore da impostare all'oggetto</param>
        /// <returns>Il risultato dell'operazione sul repository. Ritorna un valore negativo in caso di errori, altrimenti un valore
        /// che identifica l'operazione eseguita</returns>
        public abstract RepositoryResponse ArraySetElement(string key, string property, int index, string obj_prop, object value);
        /// <summary>
        /// Name of the Repository Type.
        /// This property is used by the factory to check if the right repository implementation has been loaded
        /// </summary>
        public String RepositoryType
        {
            get;
            protected set;
        }
        /// <summary>
        /// Disposes the repository
        /// </summary>
        public abstract void Dispose();
    }
}
