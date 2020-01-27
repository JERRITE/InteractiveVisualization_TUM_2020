using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    //In der liste kann man später vergleichen ob ein Objekt gezeigt wird und welchen Abstand dieses hat. Key dabei ist der name des TargetImages
    private List<BuildingObject> ht = new List<BuildingObject>();
    private Hashtable found = new Hashtable();       //besteht aus dem namen des objekt als key und einer BuildingObjectreferenz
    private Hashtable foundTracked = new Hashtable();                 //alle objekte die gematched wurden und gerade getracked

    private float angMatch = 5f;                    //winkel in dem die Karten zueinander stehen müssenum zu matchen
    private float distMatch = 10.0f;                   //distanz in dem die Karten voneinander entfernt sein dürfen um zu matchen

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //wenn zwei oder mehr Objekte getrackt werden, dann wird die methode distanceClose() aufgerufen
        if(ht.Count > 1)
        {
            distanceClose();
        }
    }

    //die methode schaut ob objekte die gerade getrackt werden das gleiche tag haben und wenn ja dann schaut sie ob distance und 
    //rotation der objekte stimmen um sie als gefunden zu markieren.
    public bool distanceClose()
    {
        for (int i = 0; i < ht.Count - 1; i++)
        {
            for (int j = i + 1; j < ht.Count; j++)
            {
                Debug.Log("bis hier kommt es " + ht[i].getTag() + "  und   " + ht[j].getTag());
                if (ht[i].getTag().Equals(ht[j].getTag()))
                {
                    BuildingObject obj1 = ht[i];
                    BuildingObject obj2 = ht[j];

                    // gibt die entfernung an wie weit die beiden objekte auseinander sind.
                    float distance = Vector3.Distance(obj1.getImageTarget().transform.position, obj2.getImageTarget().transform.position);

                    //gibt den winkel zwischen zwei rotationen zurück. 
                    float angle1 = Quaternion.Angle(obj1.getImageTarget().transform.rotation, obj2.getImageTarget().transform.rotation);
                    Debug.Log("Das ist die Distanz zwischen den Objekten:  " + distance + " und das die Roatation:  " + angle1);

                    //wen die entfernung und die rotation kleiner als der spielraum(distmatch und angmatch) 
                    //sind dann werden die objekte in found hinzugefügt 
                    Debug.Log("Die distanz der objekte ist " + distance + "--------------------------------------------------");
                    if (distance <= distMatch /*&& angle1 <= angMatch*/)
                    {
                        if (!found.ContainsKey(obj1.getName()))
                        {
                            found.Add(obj1.getName(), obj1);
                        }
                        if (!found.ContainsKey(obj2.getName()))
                        {
                            found.Add(obj2.getName(), obj2);
                        }
   
                        // objekte werden aus der getrackten aber noch nciht gefundenen liste entfernt
                        ht.RemoveAt(j);
                        ht.RemoveAt(i);

                        //Paar wird erstelltmit obj1 und zweites objekt obj2 wird hinzugefügt und dann paar in liste eingetragen
                        paarObject paarobj = new paarObject(obj1);
                        paarobj.addObj(obj2);
                        foundTracked.Add(obj1.getTag(), paarobj);

                        return true;
                    }
                }
            }
        }
        return false;
    }


    //hinzufügen eines BuildingObjects in die hastable  // 0 bedeutet false; 1 bedeutet true; und 2 bedeutet, dass das found objekt schon drinnen.
    public int addTrackedImageToList(string name, string tag, GameObject imageTarget)
    {
        
        //für die gefunden objekte 
        if (found.ContainsKey(name))
        {
            Debug.Log("Das OBJEKT wurde bereits GEFUNDEN");
            //wenn noch keins von den tags gerade getracked wurde wird komplett neues paar entstellt
            if (!foundTracked.ContainsKey(tag))
            {
                Debug.Log("NEUES PAAR wird erstellt");
                foundTracked.Add(tag, new paarObject(new BuildingObject(name, tag, imageTarget)));
                return 1;
            } else
            //sonst wird das objekt zu einem vorhandenen paar objekt hinzugefügt.
            {
                Debug.Log("VORHANDENES PAAR war bereits da und wird hinzugefügt");
                paarObject paarobj = (paarObject)foundTracked[tag];
                paarobj.addObj(new BuildingObject(name, tag, imageTarget));
                return 1;
            }

        }
        Debug.Log("Das objekt wurde noch nicht gefunden ListADD -------------------");
        //sucht object und fügt es hinzu falls noch nicht vorhanden
        for (int i = 0; i < ht.Count; i++)
        {
            if ((ht[i].getName()).Equals(name))
            {
                return 0;
            }
        }

        ht.Add(new BuildingObject(name, tag, imageTarget));
        return 1;
    }


    //entfernen eines BuildingObjects in die liste. true wenn entfernt wurde.
    public bool removeTrackedImageToList(string name, string tag)
    {
        //es wird geschaut ob das zu entferne objekt gematched wurde bereits
        if (found.ContainsKey(name))
        {
            //dann wird es nämlich vom paar entfernt 
            if (foundTracked.ContainsKey(tag))
            {
                Debug.Log("es wurde ein PAAR gefunden");
                paarObject paarobj = (paarObject)foundTracked[tag];
                if (!paarobj.removeObj(name))
                {
                    Debug.Log("PAAR war nur noch ein OBJEKT drinnen");
                    foundTracked.Remove(tag);
                    return true;
                } else
                {
                    Debug.Log("Im PAAR waren noch zwei OBJEKTE drinnen");
                    return true;
                }
            }
            return false;
        }

        //sucht object und löscht es raus
        for (int i = 0; i < ht.Count; i++)
        {
            if((ht[i].getName()).Equals(name))
            {
                ht.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    //Datenstruktur für die Images die gerade getrackted werden.
    private class BuildingObject
    {
        private string name;
        private string tag;
        private GameObject imageTarget;
        private int tracked;

        public BuildingObject(string name, string tag, GameObject imageTarget)
        {
            this.name = name;
            this.tag = tag;
            this.imageTarget = imageTarget;
        }

        public string getName()
        {
            return name;
        }
        public string getTag()
        {
            return tag;
        }
        public GameObject getImageTarget()
        {
            return imageTarget;
        }
        public int getTracked()
        {
            return tracked;
        }
        public void setTracked(int tracked)
        {
            this.tracked = tracked;
        }
    }
    //Datenstruktur für die Paare an Images
    private class paarObject
    {
        private BuildingObject obj1;
        private BuildingObject obj2;

        public paarObject(BuildingObject obj1)
        {
            this.obj1 = obj1;
            obj1.getImageTarget().GetComponent<TrackableEventHandler>().updateMood("building");
            this.obj2 = null;
        }

        public void addObj(BuildingObject obj)
        {
            if(obj2 == null)
            {
                this.obj2 = obj;
                obj2.getImageTarget().GetComponent<TrackableEventHandler>().updateMood("info");
            }
        }
        public bool removeObj(string name)
        {
            if(obj2 == null)
            {
                return false;
            } else
            {
                obj1 = obj2;
                obj1.getImageTarget().GetComponent<TrackableEventHandler>().updateMood("building");
                obj2 = null;
                return true;
            }
        }
    }
}
