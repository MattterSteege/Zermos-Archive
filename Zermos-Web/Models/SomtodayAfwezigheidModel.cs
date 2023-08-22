using System;
using System.Collections.Generic;

namespace Zermos_Web.Models.SomtodayAfwezigheidModel;

public class SomtodayAfwezigheidModel
{
    public List<Item> items { get; set; }
}

public class AbsentieMelding
{
//    public Leerling leerling { get; set; }
    public AbsentieReden absentieReden { get; set; }
    public DateTime datumTijdInvoer { get; set; }
    public DateTime beginDatumTijd { get; set; }
    public DateTime eindDatumTijd { get; set; }
    public int beginLesuur { get; set; }
    public int eindLesuur { get; set; }
    public bool afgehandeld { get; set; }
    public string opmerkingen { get; set; }
}

public class AbsentieReden
{
    public string absentieSoort { get; set; }
    public string afkorting { get; set; }
    public string omschrijving { get; set; }
    public bool geoorloofd { get; set; }
    // public bool kiesbaarDoorVerzorger { get; set; }
    // public bool verzorgerMagTijdstipKiezen { get; set; }
    // public bool verzorgerEinddatumVerplicht { get; set; }
    // public bool standaardAfgehandeld { get; set; }
//    public Vestiging vestiging { get; set; }
}

// public class Afspraak
// {
//     public AfspraakType afspraakType { get; set; }
//     public string locatie { get; set; }
//     public DateTime beginDatumTijd { get; set; }
//     public DateTime eindDatumTijd { get; set; }
//     public int beginLesuur { get; set; }
//     public int eindLesuur { get; set; }
//     public string titel { get; set; }
// }

// public class AfspraakType
// {
//     public string naam { get; set; }
//     public string omschrijving { get; set; }
//     public int standaardKleur { get; set; }
//     public string categorie { get; set; }
//     public string activiteit { get; set; }
//     public int percentageIIVO { get; set; }
//     public bool presentieRegistratieDefault { get; set; }
//     public bool actief { get; set; }
//     public Vestiging vestiging { get; set; }
// }

// public class IngevoerdDoor
// {
//     public string UUID { get; set; }
//     public int nummer { get; set; }
//     public string afkorting { get; set; }
//     public string achternaam { get; set; }
//     public string geslacht { get; set; }
//     public string voorletters { get; set; }
//     public string roepnaam { get; set; }
//     public string voorvoegsel { get; set; }
// }

public class Item
{
    public DateTime beginDatumTijd { get; set; }
    public DateTime eindDatumTijd { get; set; }
    public int beginLesuur { get; set; }
    public int eindLesuur { get; set; }
//    public string waarnemingSoort { get; set; }
//    public Leerling leerling { get; set; }
    public AbsentieMelding absentieMelding { get; set; }
//    public Afspraak afspraak { get; set; }
    public bool afgehandeld { get; set; }
    public DateTime invoerDatum { get; set; }
    public DateTime laatstGewijzigdDatum { get; set; }
    public string herkomst { get; set; }
//    public IngevoerdDoor ingevoerdDoor { get; set; }
//    public LaatstGewijzigdDoor laatstGewijzigdDoor { get; set; }
    public AbsentieReden absentieReden { get; set; }
    public string opmerkingen { get; set; }
}

// public class LaatstGewijzigdDoor
// {
//     public string UUID { get; set; }
//     public int nummer { get; set; }
//     public string afkorting { get; set; }
//     public string achternaam { get; set; }
//     public string geslacht { get; set; }
//     public string voorletters { get; set; }
//     public string roepnaam { get; set; }
//     public string voorvoegsel { get; set; }
// }

// public class Leerling
// {
//     public string UUID { get; set; }
//     public int leerlingnummer { get; set; }
//     public string roepnaam { get; set; }
//     public string voorvoegsel { get; set; }
//     public string achternaam { get; set; }
// }

// public class Vestiging
// {
//     public string naam { get; set; }
// }