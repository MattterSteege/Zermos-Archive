using System;
using System.Collections.Generic;

namespace Zermos_Web.Models.somtodayHomeworkModel
{
    public class AdditionalObjects
    {
        public SwigemaaktVinkjes swigemaaktVinkjes { get; set; }
    }

    public class Item
    {
        public AdditionalObjects additionalObjects { get; set; }
        public StudiewijzerItem studiewijzerItem { get; set; }
        public Lesgroep lesgroep { get; set; }
        public DateTime datumTijd { get; set; }
        public bool gemaakt { get; set; }
        public string UUID { get; set; }
    }

    public class Lesgroep
    {
        public Vak vak { get; set; }
    }

    public class SomtodayHomeworkModel
    {
        public List<Item> items { get; set; }
    }

    public class StudiewijzerItem
    {
        public string huiswerkType { get; set; }
        public string omschrijving { get; set; }
        public string onderwerp { get; set; }
        public List<Bijlagen> bijlagen { get; set; }
    }

    public class SwigemaaktVinkjes
    {
        public List<Item> items { get; set; }
    }

    public class Vak
    {
        public string naam { get; set; }
    }

    public class Bijlagen
    {
        public string omschrijving { get; set; }
        public List<AssemblyResult> assemblyResults { get; set; }
    }

    public class AssemblyResult
    {
        public string fileUrl { get; set; }
    }
    
//     public class SomtodayHomeworkModel
//     {
//         public List<Item> items { get; set; }
//     }
//
//     public class AdditionalObjects
//     {
//         public SwigemaaktVinkjes swigemaaktVinkjes { get; set; }
//         //public object huiswerkgemaakt { get; set; }
//         //public object studiewijzerId { get; set; }
//     }
//
//     public class AssemblyResult
//     {
//         // public List<Link> links { get; set; }
//         // public List<object> permissions { get; set; }
//         // public AdditionalObjects additionalObjects { get; set; }
//         // public string assemblyFileType { get; set; }
//         // public string fileExtension { get; set; }
//         // public string mimeType { get; set; }
//         // public double fileSize { get; set; }
//         // public string fileType { get; set; }
//         public string fileUrl { get; set; }
//         //public string sslUrl { get; set; }
//         //public string fileName { get; set; }
//     }
//
//     public class Bijlagen
//     {
//         //public List<Link> links { get; set; }
//         //public List<object> permissions { get; set; }
//         //public AdditionalObjects additionalObjects { get; set; }
//         public string omschrijving { get; set; }
//         //public UploadContext uploadContext { get; set; }
//         public List<AssemblyResult> assemblyResults { get; set; }
//         //public int sortering { get; set; }
//         //public bool zichtbaarVoorLeerling { get; set; }
//     }
//
//     public class Item
//     {
//         //public List<Link> links { get; set; }
//         //public List<Permission> permissions { get; set; }
//         public AdditionalObjects additionalObjects { get; set; }
//         public StudiewijzerItem studiewijzerItem { get; set; }
//         //public int sortering { get; set; }
//         public Lesgroep lesgroep { get; set; }
//         public DateTime datumTijd { get; set; }
//         //public DateTime aangemaaktOpDatumTijd { get; set; }
//         //public object swiToekenningId { get; set; }
//         public bool gemaakt { get; set; }
//         //public int weeknummerVanaf { get; set; }
//         public string UUID { get; set; }
//         //public int leerlingnummer { get; set; }
//         //public object roepnaam { get; set; }
//         //public object voorvoegsel { get; set; }
//         //public object achternaam { get; set; }
//     }
//
//     public class Lesgroep
//     {
//         // public List<Link> links { get; set; }
//         // public List<Permission> permissions { get; set; }
//         // public AdditionalObjects additionalObjects { get; set; }
//         // public string UUID { get; set; }
//         // public string naam { get; set; }
//         // public Schooljaar schooljaar { get; set; }
//         public Vak vak { get; set; }
//         // public bool heeftStamgroep { get; set; }
//         // public bool examendossierOndersteund { get; set; }
//         // public Vestiging vestiging { get; set; }
//     }
//
//     // public class Link
//     // {
//     //     public object id { get; set; }
//     //     public string rel { get; set; }
//     //     public string type { get; set; }
//     //     public string href { get; set; }
//     // }
//     
//     public class StudiewijzerItem
//     {
//         // public List<Link> links { get; set; }
//         // public List<Permission> permissions { get; set; }
//         // public AdditionalObjects additionalObjects { get; set; }
//         public string onderwerp { get; set; }
//         public string huiswerkType { get; set; }
//         public string omschrijving { get; set; }
//         // public bool inleverperiodes { get; set; }
//         // public bool lesmateriaal { get; set; }
//         // public bool projectgroepen { get; set; }
//         public List<Bijlagen> bijlagen { get; set; }
//         // public List<object> externeMaterialen { get; set; }
//         // public List<object> inlevermomenten { get; set; }
//         // public bool tonen { get; set; }
//         // public bool notitieZichtbaarVoorLeerling { get; set; }
//         // public string leerdoelen { get; set; }
//     }
//
//     public class SwigemaaktVinkjes
//     {
//         public List<Item> items { get; set; }
//     }
//
//     // public class UploadContext
//     // {
//     //     public List<Link> links { get; set; }
//     //     public List<object> permissions { get; set; }
//     //     public AdditionalObjects additionalObjects { get; set; }
//     //     public string fileState { get; set; }
//     //     public string assemblyId { get; set; }
//     // }
//
//     public class Vak
//     {
//         // public List<Link> links { get; set; }
//         // public List<Permission> permissions { get; set; }
//         // public AdditionalObjects additionalObjects { get; set; }
//         // public string afkorting { get; set; }
//         public string naam { get; set; }
//     }
//     
// //create item constructor

}