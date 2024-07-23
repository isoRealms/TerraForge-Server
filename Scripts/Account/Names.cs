﻿using Server.Commands;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Server
{
	public class NameList : IEnumerable<string>
	{
		private static readonly NameList[] m_Lists = new NameList[]
		{
			#region Name Lists

			#region Tokuno Male

			new("Tokuno Male", new[]
			{
				"Dosyaku", "Warimoto", "Ogino", "Soutatsu", "Wajida", "Batsu", "Hikoza", "Adai", "Ukita", "Sanesue", "Chihu", "Akiosa", "Riku",
				"Kongou", "Numata", "Kirzahn", "Gakudai", "Nakai", "Ukita", "Ashimaru", "Fuha", "Gakudai", "Numata", "Waricika", "Chihu",
				"Sanesue", "Yose", "Shinsirou", "Yasuke", "Akisuke", "Naozumi", "Numatta", "Kichizo", "Nasuno", "Akaba", "Arikoto", "Kengi",
				"Magosuke",
			}),

			#endregion

			#region Tokuno Female

			new("Tokuno Female", new[]
			{
				"Ahara", "Yume", "Ayane", "Yuki", "Wakae", "Akala", "Ikuha", "Wayu", "Haru", "Zannia", "Shino", "Agate", "Akashi", "Niji",
				"Yochika", "Toyoka", "Wayo", "Iroha", "Nakai", "Ayatsumi", "Wayu", "Yumiha", "Enji", "Aasin", "Akela", "Chiwa", "Suni", "Wakumi",
				"Ruri", "Tama",
			}),

			#endregion

			#region Elf Female

			new("Elf Female", new[]
			{
				"Besran", "Caraphrestol", "Edhelarth", "Fiaro", "Fisonna", "Ilithia", "Jealian", "Kiliatha", "Merilcheleg", "Nelfael", "Nimbilin",
				"Silivin", "Versal",
			}),

			#endregion

			#region Elf Male

			new("Elf Male", new[]
			{
				"Azayn", "Arnen", "Coribrar", "Daegalen", "Dhoudrim", "Elwing", "Gilthathar", "Hanchall", "Kandrialis", "Koliumder", "Laeroll",
				"Nemin", "Ormaen", "Theninorn",
			}),

			#endregion

			#region Gargoyle Female

			new("Gargoyle Female", new[]
			{
				"Ansikart", "Ane'Ska", "Artaios", "Aurvidlem", "Betra", "Forskis", "Inmanilem", "Khalora", "Quaeven", "Runeb", "Silamo", "Sil'Cor",
				"Tir'Pena", "T'Lem", "Zu'Teika",
			}),

			#endregion

			#region Gargoyle Male

			new("Gargoyle Male", new[]
			{
				"Anmanivas", "Au'Rek", "Bolesh", "Draxinusom", "Foranamo", "Ga'Kop", "G'Krill", "Hir'Roffe", "Inwisloklem", "Naxatilor", "Sai'Gem",
				"Sin'Vraal", "Teregus", "Valkadesh", "Zoterios",
			}),

			#endregion

			#region Bird

			new("Bird", new[]
			{
				"a wren", "a swallow", "a warbler", "a nuthatch", "a chickadee", "a thrush", "a nightingale", "a starling", "a skylark", "a finch",
				"a crossbill", "a sparrow", "a towhee", "a woodpecker", "a kingfisher", "a tern", "a plover", "a lapwing", "a hawk", "a cuckoo",
				"a swift",
			}),

			#endregion

			#region Ancient Lich

			new("Ancient Lich", new[]
			{
				"Kaltivel", "Anshu", "Maliel", "Baratoz", "Almonjin",
			}),

			#endregion

			#region Balron

			new("Balron", new[]
			{
				"the Lord of the Abyss", "the Collector of Souls", "the Slayer",
			}),

			#endregion

			#region Darknight Creeper

			new("Darknight Creeper", new[]
			{
				"Pariah", "Ydoc Llessue", "Zhaan", "Lorbna", "Gragok", "Thranger", "Krygar", "Grothelfiend", "Centibis", "Farthak", "Laitesach",
				"Crenabil", "Krullus", "Legron", "Noirkrach", "Blassarrabb", "Gragragron", "Vendodroth", "Flaggroth", "Vilithrar",
			}),

			#endregion

			#region Impaler

			new("Impaler", new[]
			{
				"Po-Kor", "Manglar", "Verolyn", "Gathfe", "Skred", "Flandrith", "Stavinfeks", "Steelbane", "Crarigor", "Empalk", "Perfus",
				"Cassiel", "Magor", "Xtul", "Vladeer", "Scrill", "Slix", "Ix", "Selminus", "Victux",
			}),

			#endregion

			#region Demon Knight

			new("Demon Knight", new[]
			{
				"Hrallath", "Heksen", "Peinsluth", "Keelus", "Kra'an", "Ankou", "Turi'el", "Azazel", "Armarus", "Grigorus", "Ga'ahp", "Therion",
				"Peirazo", "Ponerus", "Arhaios Ophis", "Vairocan", "Arsat", "Karnax", "Taet Nu'uhn",
			}),

			#endregion

			#region Shadow Knight

			new("Shadow Knight", new[]
			{
				"Oghmus", "Arametheus", "Terxor", "Erdok", "Archatrix", "Jonar", "Marth'Fador", "Helzigar", "Tyrnak", "Krakus", "Marcus Fel",
				"Lord Kaos", "Doomor", "Uhn", "Malashim", "Samael", "Nelokhiel", "Montobulus", "Usuhl", "Zul",
			}),

			#endregion

			#region Fire Gargoyle

			new("Fire Gargoyle", new[]
			{
				"a fiery gargoyle", "a burning gargoyle", "a smoldering gargoyle", "a blistering gargoyle", "a sweltering gargoyle", "a flaming gargoyle",
				"a scorching gargoyle", "a blazing gargoyle", "a searing gargoyle",
			}),

			#endregion

			#region Gargoyle Vendor

			new("Gargoyle Vendor", new[]
			{
				"Ansiraal", "Aurlem", "Betra", "Draxsom", "Fodus", "Forbrak", "ForLem", "Horffe", "Inforlem", "Invis", "Laplem", "Naxator",
				"Quaeven", "Sarpling", "Silamo", "Tereg", "Volesh", "Vraal", "WisSur",
			}),

			#endregion

			#region Gargoyle Quester

			new("Gargoyle Quester", new[]
			{
				"Agralem", "Beninort", "Ansikart", "Aurvidlem",
			}),

			#endregion

			#region Centaur

			new("Centaur", new[]
			{
				"Sophanon", "Caryax", "Daemeox", "Phlegon", "Aerophus", "Euforus", "Pallax", "Nikaon", "Licouax", "Lindsaon", "Bastax", "Magdanon",
				"Thayax", "Aethon", "Ceridus", "Galenon", "Rhysus", "Auramax", "Aldrax", "Anaxus", "Luceus", "Quarax", "Ariax", "Balarax",
				"Vincenus", "Loxias", "Birhamus", "Lekax", "Nyctinus", "Myrsinus",
			}),

			#endregion

			#region Ethereal Warrior

			new("Ethereal Warrior", new[]
			{
				"Galdrion", "Briellis", "Charpris", "Jesurian", "Agrast", "Beldrion", "Polis", "Arafel", "Melestoref", "Lanculis", "Judaselo",
				"Pietrov",
			}),

			#endregion

			#region Pixie

			new("Pixie", new[]
			{
				"Klian", "Klistra", "Laeri", "Ciline", "Shiale", "Ourie", "Piepe", "Liera", "Sili", "Sefi", "Cynthe", "Nedra", "Hali", "Jiki",
				"Piku", "Rael", "Zanne", "Zut", "Sini", "Os", "Wienne", "Xian", "Ybri", "Calee", "Shendri", "Shri",
			}),

			#endregion

			#region Savage

			new("Savage", new[]
			{
				"Kahl", "Ghrom", "Ogger", "Vek", "Sai'ge", "Groov'h", "Khaonem", "Malen", "Atar", "Aicee", "Vaype", "Halex", "Yar", "Hanz",
				"Evocah", "Bishor", "Jelak", "Adrok", "Praphut", "Usile", "Sann", "Sabba", "Prie", "Atuk", "Chaca", "ShoJo", "Baccu", "Bonkie",
				"Alli", "Jexa", "La'Loh", "Arda", "Cordee", "Lana", "Lar", "Araka", "Rhunda", "Squee", "Bhora", "Niha", "Olaufee", "Sinthe",
				"Karawyn", "Gruwulf", "Masena", "Nasha", "Sargaza",
			}),

			#endregion

			#region Savage Rider

			new("Savage Rider", new[]
			{
				"Kahl", "Ghrom", "Ogger", "Vek", "Sai'ge", "Groov'h", "Khaonem", "Malen", "Atar", "Aicee", "Vaype", "Halex", "Yar", "Hanz",
				"Evocah", "Bishor", "Jelak", "Adrok", "Praphut", "Usile", "Sann", "Sabba", "Prie", "Atuk", "Chaca", "ShoJo", "Baccu", "Bonkie",
				"Alli",
			}),

			#endregion

			#region Savage Shaman

			new("Savage Shaman", new[]
			{
				"Jexa", "La'Loh", "Arda", "Cordee", "Lana", "Lar", "Araka", "Rhunda", "Squee", "Bhora", "Niha", "Olaufee", "Sinthe", "Karawyn",
				"Gruwulf", "Masena", "Nasha", "Sargaza",
			}),

			#endregion

			#region Golem Controller

			new("Golem Controller", new[]
			{
				"Zelik", "Kronos", "Zakron", "Velis", "Chujil", "Hygraph", "Dyntrall", "Zarus", "Phoseph", "Malkavik", "Zevras", "Vakel",
				"Daklon", "Zamog", "Tavurk", "Drakov", "Zazik", "Yyntrix", "Zazik", "Fropoz", "Noxtrag", "Makzok", "Galzan", "Drakan", "Drakzik",
				"Vazmog",
			}),

			#endregion

			#region Daemon

			new("Daemon", new[]
			{
				"Aamon", "Agalierept", "Agares", "Aglasis", "Aiwaz", "Astaroth", "Ayperos", "Azatoth", "Azmodaeus", "Azrael", "Ba'al", "Baal",
				"Barbatos", "Bathim", "Bathsin", "Be'elzebub", "Be'elzebubba", "Bechard", "Beelzebuth", "Botis", "Brulefer", "Bucon", "Buer",
				"Clauneck", "Clitheret", "Cthulhu", "Druzil", "El Chupacabra", "Eleogap", "Eliezer", "Eligor", "Eracove", "Faraii", "Fleurety",
				"Frimost", "Frucissiere", "Fruitimiere", "Glassyalabolas", "Guland", "Gusoyn", "Hael", "Haristum", "Heramael", "Hiepacth",
				"Huictiigara", "Humots", "Khil", "Maleki", "Marbas", "Mephistopheles", "Mersilde", "Minoson", "Moloch", "Molech", "Morail",
				"Musisin", "Naberrs", "Nebiros", "Nebirots", "Nyarlathotep", "Pentagnony", "Proculo", "Pruslas", "Pursan", "Rofocale", "Sargatans",
				"Satanchia", "Satanciae", "Segal", "Sergulath", "Sergutthy", "Sidragrosam", "Sirchade", "Surgat", "Sustugriel", "Tarchimache",
				"Tarihimal", "Trimasel", "Vaelfar", "Wormius", "Yog-Sothoth", "Y'reif Eci", "Zoray",
			}),

			#endregion

			#region Evil Mage Lord

			new("Evil Mage Lord", new[]
			{
				"Xarot the Black Archmage", "Kwan Li the Lord of the Mists", "Bazerion the Wizard-Lord", "Ylthallynon the Insane",
			}),

			#endregion

			#region Evil Mage

			new("Evil Mage", new[]
			{
				"Ronlyn", "Merkul", "Zasfus", "Zain", "Doraghir", "Danaghir", "Staylin", "Kraylin", "Limnoch", "Kranor", "Kraenor", "Kranostil",
				"Kranostir", "Lilithack", "Terus", "Thaelin", "Thulack", "Jiltharis", "Garigor", "Banothil", "Bainothil", "Quain", "Ilzinias",
				"Mardis the Avenger", "Phalil the Unexpected", "Lord Adnoc", "Leje the Invincible", "Master Akris", "Sartan", "Zejron the Wild",
				"Pitt the Elder", "Puzilan the Puzzler", "Vantrom", "Singhe-Dul", "Tyrin Kuhl", "Xarot the Black", "Kwan Li", "Lord of the Mists",
				"Viktor Blackoak", "Odilion", "Raith", "Lord of Rats", "Bazerion", "the Wizard-Lord", "Tybevriat Varn", "Keldor the Modoc",
				"Magelord Varsan", "Izlay the Ebonheart", "Slirith Iceblood", "Alcor", "Vitar", "Feenark", "Sang Qui", "Lyticant", "Aegnor",
				"Aelfric", "Ainvar", "Arazion", "Ardarion", "Arkanis", "Athrax", "Barghest", "Baros", "Begarin", "Beynlore", "Burat", "Cairne",
				"Carthon", "Chamdar", "Ciric", "Cruzado", "Cyphrak", "D'Harun", "Daelon", "Daktar", "Darvain", "Dracus", "Dradar", "Draelin",
				"Draenor", "Durodund", "Eklor", "Elrak", "Fangorn", "Farynthane", "Galtor", "Gemma", "Gragus", "Irian", "Israfel", "Jaden",
				"Jefahn", "K'shar", "Kalimus", "Kallomane", "Kanax", "Kelnos", "Kharn", "Khir", "Kragon", "Kylnath", "Larac", "Lathis", "Lenroc",
				"Lonthorynthoryl", "Lorreck", "Mattrick", "Mazrim", "Mazrim", "Modrei", "Morturus", "Muktar", "Murdon", "Murron", "Myndon",
				"Mythran", "Mytor", "Nabius", "Nalynkal", "Nazgul", "Paorin", "Quillan", "Rendar", "Scythyn", "Shilor", "Sobran", "Soltak",
				"Sorz", "Taban", "Telzar", "Teron", "Trethovian", "Tyrnar", "Ulath", "Vandor", "Vermithrax", "Vlade", "Volan", "Wydstrin",
				"X'calak", "Xaelin", "Xandor", "Xarthos", "Xaxtox", "Xenix", "Xiltanth", "Xylor", "Xystil", "Yazad", "Yllthane", "Ylthallynon",
				"Ythoryn", "Zalifar", "Zathrix", "Zunrek",
			}),

			#endregion

			#region Ratman

			new("Ratman", new[]
			{
				"Ccketakiki", "Chachak", "Chachaktak", "Chackuk", "Chak", "Chaki", "Chaki", "Chakreki", "Chaktuki", "Chakukki", "Charitiki",
				"Chatuki", "Chectik", "Chectik", "Chek", "Chekeckaki", "Cheki", "Chekkakii", "Cherek", "Chetak", "Chetickuki", "Chiackukk",
				"Chichachak", "Chichak", "Chichak", "Chichoki", "Chichoki", "Chickek", "Chickek", "Chickeki", "Chickeki", "Chickekiaki", "Chikavi",
				"Chikchickeki", "Chikckak", "Chikek", "Chiketckuki", "Chiki", "Chikitchaki", "Chiktaki", "Chiritchek", "Chitaviok", "Chokchak",
				"Chokirek", "Chotechiki", "Ckak", "Ckek", "Ckekckuki", "Ckeki", "Ckekickuk", "Ckikhiki", "Ckikicheki", "Ckukchik", "Ckukichek",
				"Ctiktik", "Dachek", "Dackatuki", "Dactuk", "Dafactik", "Deckarreki", "Deektuk", "Defetav", "Dekckuk", "Detckiki", "Detckuki",
				"Detik", "Dicchok", "Dickiki", "Dikfachok", "Ditecckek", "Diwarek", "Eachik", "Eactiki", "Ecckkekek", "Eckaki", "Eckechakiki",
				"Ecketak", "Ecterek", "Ectuk", "Eekckuk", "Eektuk", "Eicchiki", "Eickuki", "Ekiuki", "Etakheki", "Etavchiki", "Etckuki", "Etik",
				"Fachchekiok", "Fachok", "Fackak", "Fackek", "Fackik", "Factavi", "Factik", "Fecckik", "Fechaki", "Feractav", "Fetav", "Fetckiki",
				"Firchik", "Firecheki", "Fireki", "Fitactaki", "Fitaki", "Fitcheki", "Frecckeki", "Hekckeki", "Hiki", "Hikitchaki", "Hokchek",
				"Ikchaki", "Iki", "Kackak", "Kactavi", "Kadicchok", "Kakhoki", "Kaki", "Kakiki", "Karrekichoki", "Katukickek", "Kecckik",
				"Kechaki", "Kekachek", "Kekachik", "Kekkik", "Kicchiki", "Kichak", "Kickak", "Kidikiki", "Kietik", "Kik", "Kikechokii", "Kikiaki",
				"Kiktaki", "Kirchik", "Kireki", "Kireki", "Kitak", "Kitaki", "Kitavi", "Kitcheki", "Ktukchok", "Kukeckaki", "Kuki", "Kukiecckak",
				"Rackek", "Ractav", "Ratitchaki", "Recckeki", "Recheki", "Rekchectik", "Reki", "Rektav", "Rektavi", "Retchectik", "Reteckaki",
				"Retituki", "Rikchickek", "Rikecckak", "Ritchek", "Ritchekckiki", "Ritiki", "Ritikituk", "Tackik", "Tactaki", "Tactikiv",
				"Tadectuk", "Tak", "Tak", "Taki", "Taki", "Taktav", "Tavchichoki", "Taviactak", "Taviectuk", "Tecckek", "Techiki", "Techikickik",
				"Teckak", "Tedetik", "Tekactiki", "Tekchichak", "Tickukitaki", "Tictak", "Tidetckuki", "Tikckek", "Tikickeki", "Titchaki",
				"Tituki", "Tukckaki", "Tukitiki", "Tukituki", "Vachichak", "Vackuk", "Vactak", "Vaveckaki", "Vechoki", "Vectaki", "Vevactak",
				"Vevitavi", "Vitavi", "Vitchak", "Vitituki", "Vivackuk", "Vivitchak", "Vovechoki", "Warek", "Warreki", "Wecckak", "Weckaki",
				"Wedachek", "Wikchichoki", "Wochickeki",
			}),

			#endregion

			#region Lizardman

			new("Lizardman", new[]
			{
				"Aissssaiss", "Aisyths", "Alasthsiyss", "Alessitsl", "Alsiths", "Alssi", "Alsyth", "Asahlysy", "Asathsaisss", "Asaysth", "Asiashy",
				"Asistsais", "Asitssis", "Asiyss", "Astha", "Asthcieth", "Astysah", "Athcesth", "Athys", "Athysthes", "Atslah", "Aysthyss",
				"Cesthaysth", "Cetys", "Ciethlish", "Citsy", "Cythshi", "Cytsi", "Ekthsisthh", "Essith", "Estha", "Etys", "Halasth", "Haless",
				"Halsasah", "Hasits", "Hekths", "Hiiths", "Hiitsl", "Hissis", "Hlyiss", "Hlylylyshi", "Hlyshilyly", "Hlysylyiss", "Hlythilsyth",
				"Hissalsthy", "Hysslssi", "Hsysylh", "Hthessthil", "Hthissy", "Htisthh", "Htsiathys", "Hyisass", "Hysil", "Hysyiss", "Iasia",
				"Icythhlysy", "Iithsissh", "Iitsliss", "Ilshy", "Ilsyth", "Isalraaat", "Isassthys", "Isathys", "Isiss", "Isissil", "Isshi",
				"Isshlshy", "Isstha", "Issthas", "Issthyl", "Issysh", "Issyt", "Isthhtis", "Istis", "Istththh", "Isyts", "Ithstsesh", "Ithsy",
				"Iththis", "Itsesh", "Itshas", "Itsltlish", "Itsthil", "Itsylh", "Iyssysil", "Kthystshas", "Kthythes", "Lasath", "Lastha",
				"Lasthtsthih", "Lesstslah", "Lestha", "Lilhals", "Lilsyth", "Lishaless", "Lishath", "Lissysh", "Lissyt", "Lithekths", "Litsylh",
				"Llithlish", "Lshyshas", "Lssissi", "Lsthyssy", "Lsyth", "Lsythstha", "Lyissisis", "Lylsthy", "Lylyisass", "Lyshiisal", "Lyssthih",
				"Lyssthy", "Lysyiitsl", "Lythiiiths", "Lytsthih", "Saisalasths", "Saishtsi", "Saisssmyss", "Salsesh", "Salssi", "Sasisthasth",
				"Sasss", "Sasth", "Sath", "Sathsthih", "Sathys", "Satslah", "Saysth", "Scesthhals", "Seshsthyl", "Sessith", "Shasits", "Shassthy",
				"Shisis", "Shitha", "Shysil", "Shythis", "Siasia", "Siaththh", "Sicyth", "Silshy", "Silthyl", "Sisal", "Sisshi", "Sisshlythi",
				"Sistlilh", "Sithtththh", "Sitsesh", "Sitshas", "Sitstsi", "Siyss", "Slahyisass", "Slasath", "Slishless", "Ssaisss", "Ssasist",
				"Sshiscesth", "Ssisal", "Ssithsiss", "Ssiyth", "Sssaisss", "Ssssshi", "Ssthasssthas", "Ssthihssthy", "Ssthssss", "Ssthyl",
				"Ssthyssths", "Ssyraaath", "Ssysah", "Ssyshstha", "Ssyssith", "Ssytsth", "Stasah", "Sthasth", "Sthasthas", "Sthasthih", "Sthihaisss",
				"Sthihasits", "Sthilsthy", "Sthlyly", "Sthlyshi", "Sthlysy", "Sthlythi", "Sthmissa", "Sthmyss", "Sthsasist", "Sthyss", "Sthyasia",
				"Sthycyth", "Sthyltasah", "Stissh", "Stththh", "Stysah", "Sycieth", "Sylhsths", "Syllith", "Syraaat", "Syshsyts", "Syslish",
				"Syththil", "Sythysth", "Sytlilh", "Sytlish", "Sytsyth", "Sytsyts", "Syys", "Tasahasath", "Thalasths", "Thaless", "Thals",
				"Thatsylh", "Thays", "Thekths", "Theshthis", "Thiiths", "Thiitsl", "Thilhlyshi", "Thishthes", "Thlyiss", "Thlyly", "Thlyshi",
				"Thlysy", "Thlythi", "Thmissa", "Thmyss", "Thsais", "Thslish", "Ththes", "Ththhllith", "Ththis", "Thtisthh", "Thyisass", "Thylmissa",
				"Thys", "Thysslah", "Tisiss", "Tissyth", "Tisthhhsy", "Tlilhlasth", "Tlishkths", "Tseshsath", "Tshassasisth", "Tsicyth", "Tsitys",
				"Tslahsits", "Tsthihssthih", "Tsthil", "Tsycieth", "Tsylhssyt", "Tsytysah", "Tththhsyt", "Tysahyraaath", "Tyscesth", "Tysycieth",
				"Yciethhlyly", "Yisasshmyss", "Yisstisthh", "Yllith", "Ylsthy", "Yraaathlythi", "Ysahlith", "Ysasth", "Ysath", "Yscesth",
				"Yshmissa", "Yshtsi", "Ysilsia", "Yslish", "Yssasisth", "Ysssal", "Yssthih", "Yssths", "Yssthy", "Ystasah", "Ysthsith", "Ystsy",
				"Ythals", "Ythssysh", "Ytlilh", "Ytlish", "Ytsais", "Ytssysh", "Ytsthih",
			}),

			#endregion

			#region Orc

			new("Orc", new[]
			{
				"Abghat", "Adgulg", "Aghed", "Agugh", "Aguk", "Almthu", "Alog", "Ambilge", "Apaugh", "Argha", "Argigoth", "Argug", "Arpigig",
				"Auhgan", "Azhug", "Bagdud", "Baghig", "Bahgigoth", "Bahgigoth", "Bandagh", "Barfu", "Bargulg", "Baugh", "Bidgug", "Bildud",
				"Bilge", "Bog", "Boghat", "Bogugh", "Borgan", "Borug", "Braugh", "Brougha", "Brugagh", "Bruigig", "Buadagh", "Buggug", "Builge",
				"Buimghig", "Bulgan", "Bumhug", "Buomaugh", "Buordud", "Burghed", "Buugug", "Cabugbu", "Cagan", "Carguk", "Carthurg", "Clog",
				"Corgak", "Crothu", "Cubub", "Cukgilug", "Curbag", "Dabub", "Dakgorim", "Dakgu", "Dalthu", "Darfu", "Deakgu", "Dergu", "Derthag",
				"Digdug", "Diggu", "Dilug", "Ditgurat", "Dorgarag", "Dregu", "Dretkag", "Drigka", "Drikdarok", "Drutha", "Dudagog", "Dugarod",
				"Dugorim", "Duiltag", "Durbag", "Eagungad", "Eggha", "Eggugat", "Egharod", "Eghuglat", "Eichelberbog", "Ekganit", "Epkagut",
				"Ergoth", "Ertguth", "Ewkbanok", "Fagdud", "Faghig", "Fandagh", "Farfu", "Farghed", "Fargigoth", "Farod", "Faugh", "Feldgulg",
				"Fidgug", "Filge", "Fodagog", "Fogugh", "Fozhug", "Frikug", "Frug", "Frukag", "Fubdagog", "Fudhagh", "Fupgugh", "Furbog",
				"Futgarek", "Gaakt", "Garekk", "Gelub", "Gholug", "Gilaktug", "Ginug", "Gnabadug", "Gnadug", "Gnalurg", "Gnarg", "Gnarlug",
				"Gnorl", "Gnorth", "Gnoth", "Gnurl", "Golag", "Golag", "Golub", "Gomatug", "Gomoku", "Gorgu", "Gorlag", "Grikug", "Grug",
				"Grug", "Grukag", "Grukk", "Grung", "Gruul", "Guag", "Gubdagog", "Gudhagh", "Gug", "Gug", "Gujarek", "Gujek", "Gujjab", "Gulm",
				"Gulrn", "Gunaakt", "Gunag", "Gunug", "Gurukk", "Guthakug", "Guthug", "Gutjja", "Hagob", "Hagu", "Hagub", "Haguk", "Hebub",
				"Hegug", "Hibub", "Hig", "Hogug", "Hoknath", "Hoknuk", "Hokulk", "Holkurg", "Horknuth", "Hrolkug", "Hugagug", "Hugmug", "Hugolm",
				"Ig", "Igmut", "Ignatz", "Ignorg", "Igubat", "Igug", "Igurg", "Ikgnath", "Ikkath", "Inkathu", "Inkathurg", "Isagubat", "Jogug",
				"Jokgagu", "Jolagh", "Jorgagu", "Jregh", "Jreghug", "Jugag", "Jughog", "Jughragh", "Jukha", "Jukkhag", "Julakgh", "Kabugbu",
				"Kagan", "Kaghed", "Kahigig", "Karfu", "Karguk", "Karrghed", "Karrhig", "Karthurg", "Kebub", "Kegigoth", "Kegth", "Kerghug",
				"Kertug", "Kilug", "Klapdud", "Klapdud", "Klog", "Klughig", "Knagh", "Knaraugh", "Knodagh", "Knorgh", "Knuguk", "Knurigig",
				"Kodagog", "Kog", "Kogan", "Komarod", "Korgak", "Korgulg", "Koughat", "Kraugug", "Krilge", "Krothu", "Krouthu", "Krugbu",
				"Krugorim", "Kubub", "Kugbu", "Kukgilug", "Kulgha", "Kupgugh", "Kurbag", "Kurmbag", "Laghed", "Lamgugh", "Mabub", "Magdud",
				"Malthu", "Marfu", "Margulg", "Mazhug", "Meakgu", "Mergigoth", "Milug", "Mudagog", "Mugarod", "Mughragh", "Mugorim", "Murbag",
				"Naghat", "Naghig", "Naguk", "Nahgigoth", "Nakgu", "Narfu", "Nargulg", "Narhbub", "Narod", "Neghed", "Nehrakgu", "Nildud",
				"Nodagog", "Nofhug", "Nogugh", "Nomgulg", "Noogugh", "Nugbu", "Nughilug", "Nulgha", "Numhug", "Nurbag", "Nurghed", "Oagungad",
				"Oakgu", "Obghat", "Oggha", "Oggugat", "Ogharod", "Oghuglat", "Oguk", "Ohomdud", "Ohulhug", "Oilug", "Okganit", "Olaghig",
				"Olaugh", "Olmthu", "Olodagh", "Olog", "Omaghed", "Ombilge", "Omegugh", "Omogulg", "Omugug", "Onog", "Onubub", "Onugug", "Oodagh",
				"Oogorim", "Oogugbu", "Oomigig", "Opathu", "Opaugh", "Opeghat", "Opilge", "Opkagut", "Opoguk", "Oquagan", "Orgha", "Orgoth",
				"Orgug", "Orpigig", "Ortguth", "Otugbu", "Ougha", "Ougigoth", "Ouhgan", "Owkbanok", "Paghorim", "Pahgigoth", "Pahgorim", "Pakgu",
				"Parfu", "Pargu", "Parhbub", "Parod", "Peghed", "Pehrakgu", "Pergu", "Perthag", "Pigdug", "Piggu", "Pitgurat", "Podagog",
				"Pofhug", "Pomgulg", "Poogugh", "Porgarag", "Pregu", "Pretkag", "Prigka", "Prikdarok", "Prutha", "Pughilug", "Puiltag", "Purbag",
				"Qog", "Quadagh", "Quilge", "Quimghig", "Quomaugh", "Quordud", "Quugug", "Raghat", "Raguk", "Rakgu", "Rarfu", "Rebub", "Rilug",
				"Rodagog", "Rogan", "Romarod", "Routhu", "Rugbu", "Rugorim", "Rurbag", "Rurigig", "Sabub", "Saghig", "Sahgigoth", "Sahgorim",
				"Sakgu", "Salthu", "Saraugug", "Sarfu", "Sargulg", "Sarhbub", "Sarod", "Sbghat", "Seakgu", "Sguk", "Shomdud", "Shulhug", "Sildud",
				"Silge", "Silug", "Sinsbog", "Slaghig", "Slapdud", "Slaugh", "Slodagh", "Slog", "Slughig", "Smaghed", "Smegugh", "Smogulg",
				"Snog", "Snubub", "Snugug", "Sodagh", "Sog", "Sogorim", "Sogugbu", "Sogugh", "Sombilge", "Somigig", "Sonagh", "Sorgulg", "Sornaraugh",
				"Soughat", "Spathu", "Speghat", "Spilge", "Spoguk", "Squagan", "Stugbu", "Sudagog", "Sugarod", "Sugbu", "Sugha", "Sugigoth",
				"Sugorim", "Suhgan", "Sulgha", "Sulmthu", "Sumhug", "Sunodagh", "Sunuguk", "Supaugh", "Supgugh", "Surbag", "Surgha", "Surghed",
				"Surgug", "Surpigig", "Tagdud", "Taghig", "Tandagh", "Tandagh", "Tarfu", "Targhed", "Targigoth", "Tarod", "Taugh", "Taugh",
				"Teldgulg", "Tidgug", "Tidgug", "Tilge", "Todagog", "Tog", "Toghat", "Togugh", "Torgan", "Torug", "Tozhug", "Traugh", "Trilug",
				"Trougha", "Trugagh", "Truigig", "Tuggug", "Tulgan", "Turbag", "Turge", "Ug", "Ugghra", "Uggug", "Ughat", "Ulgan", "Ulmragha",
				"Ulmrougha", "Umhra", "Umragig", "Umruigig", "Ungagh", "Unrugagh", "Urag", "Uraugh", "Urg", "Urgan", "Urghat", "Urgran", "Urlgan",
				"Urmug", "Urug", "Urulg", "Vabugbu", "Vagan", "Vagrungad", "Vagungad", "Vakgar", "Vakgu", "Vakmu", "Valthurg", "Vambag", "Vamugbu",
				"Varbu", "Varbuk", "Varfu", "Vargan", "Varguk", "Varkgorim", "Varthurg", "Vegum", "Vergu", "Verlgu", "Verthag", "Verthurg",
				"Vetorkag", "Vidarok", "Vigdolg", "Vigdug", "Viggu", "Viggulm", "Viguka", "Vitgurat", "Vitgut", "Vlog", "Vlorg", "Vorgak",
				"Vorgarag", "Vothug", "Vregu", "Vretkag", "Vrigka", "Vrikdarok", "Vrogak", "Vrograg", "Vrothu", "Vruhag", "Vrutha", "Vubub",
				"Vugub", "Vuiltag", "Vukgilug", "Vultog", "Vulug", "Vurbag", "Wakgut", "Wanug", "Wapkagut", "Waruk", "Wauktug", "Wegub", "Welub",
				"Wholug", "Wilaktug", "Wingloug", "Winug", "Woabadug", "Woggha", "Woggugat", "Woggugat", "Wogharod", "Wogharod", "Woghuglat",
				"Woglug", "Wokganit", "Womkug", "Womrikug", "Wonabadug", "Worthag", "Wraog", "Wrug", "Wrukag", "Wrukaog", "Wubdagog", "Wudgh",
				"Wudhagh", "Wudugog", "Wuglat", "Wumanok", "Wumkbanok", "Wurgoth", "Wurmha", "Wurtguth", "Wurthu", "Wutgarek", "Xaakt", "Xago",
				"Xagok", "Xagu", "Xaguk", "Xarlug", "Xarpug", "Xegug", "Xepug", "Xig", "Xnath", "Xnaurl", "Xnurl", "Xoknath", "Xokuk", "Xolag",
				"Xolkug", "Xomath", "Xomkug", "Xomoku", "Xonoth", "Xorag", "Xorakk", "Xoroku", "Xoruk", "Xothkug", "Xruul", "Xuag", "Xug",
				"Xugaa", "Xugag", "Xugagug", "Xugar", "Xugarf", "Xugha", "Xugor", "Xugug", "Xujarek", "Xuk", "Xulgag", "Xunaakt", "Xunag",
				"Xunug", "Xurek", "Xurl", "Xurug", "Xurukk", "Xutag", "Xuthakug", "Xutjja", "Yaghed", "Yagnar", "Yagnatz", "Yahg", "Yahigig",
				"Yakgnath", "Yakha", "Yalakgh", "Yargug", "Yegigoth", "Yegoth", "Yerghug", "Yerug", "Ymafubag", "Yokgagu", "Yokgu", "Yolmar",
				"Yonkathu", "Yregh", "Yroh", "Ysagubar", "Yughragh", "Yugug", "Yugug", "Yukgnath", "Yukha", "Yulakgh", "Yunkathu", "Zabghat",
				"Zabub", "Zaghig", "Zahgigoth", "Zahgorim", "Zalthu", "Zaraugug", "Zarfu", "Zargulg", "Zarhbub", "Zarod", "Zeakgu", "Zguk",
				"Zildud", "Zilge", "Zilug", "Zinsbog", "Zlapdud", "Zlog", "Zlughig", "Zodagh", "Zog", "Zogugbu", "Zogugh", "Zombilge", "Zonagh",
				"Zorfu", "Zorgulg", "Zorhgigoth", "Zornaraugh", "Zoughat", "Zudagog", "Zugarod", "Zugbu", "Zugorim", "Zuhgan", "Zulgha", "Zulmthu",
				"Zumhug", "Zunodagh", "Zunuguk", "Zupaugh", "Zupgugh", "Zurbag", "Zurgha", "Zurghed", "Zurgug", "Zurpigig",
			}),

			#endregion

			#region Female

			new("Female", new[]
			{
				"Aba", "Abby", "Abella", "Abey", "Abigail", "Abina", "Abiona", "Abira", "Abra", "Abrah", "Absinthe", "Acacia", "Acanit", "Acantha",
				"Accalia", "Acelin", "Achen", "Ada", "Adalia", "Adara", "Addi", "Adelaide", "Adele", "Adelia", "Adeline", "Adelle", "Adena",
				"Aderes", "Adesina", "Adie", "Adimina", "Adiva", "Adoncia", "Adonia", "Adora", "Adrienne", "Aelina", "Afina", "Afra", "Afrika",
				"Afton", "Agate", "Agatha", "Agnes", "Ahara", "Ahave", "Ahimsa", "Aida", "Aiella", "Aiko", "Aila", "Aileen", "Ailsa", "Aimee",
				"Ain", "Aina", "Ainhoa", "Ainsley", "Aintzane", "Airlia", "Aisling", "Aislinn", "Aithne", "Aiyana", "Ajara", "Ajay", "Ajinora",
				"Akako", "Akala", "Akanke", "Akasma", "Akela", "Akilah", "Akili", "Akilina", "Akina", "Alaina", "Alake", "Alala", "Alamanada",
				"Alana", "Alani", "Alanna", "Alaqua", "Alavda", "Alazne", "Alberta", "Albinka", "Alcina", "Aldea", "Aldercy", "Aleka", "Alenne",
				"Alesia", "Alessa", "Alethea", "Alexa", "Alexandra", "Alexandria", "Alexandrina", "Alexis", "Ali", "Alia", "Alice", "Alicia",
				"Alida", "Alike", "Alima", "Alina", "Alison", "Alita", "Alix", "Aliz", "Aliza", "Allele", "Alligra", "Allinora", "Allison",
				"Allyn", "Alma", "Alodie", "Aloysia", "Althea", "Alula", "Alumit", "Alvina", "Alvita", "Alysa", "Alyssa", "Alyssand", "Alzena",
				"Ama", "Amabel", "Amadi", "Amadika", "Amadis", "Amaia", "Amala", "Amalia", "Amanda", "Amandine", "Amara", "Amarande", "Amarante",
				"Amaris", "Amata", "Ambar", "Amber", "Ambika", "Ambis", "Ameerah", "Amelia", "Amelina", "Amethyst", "Amie", "Amiella", "Amina",
				"Aminta", "Amissa", "Amita", "Amity", "Amoke", "Amy", "Ananda", "Anastasia", "Ancelin", "Andi", "Andra", "Andraianna", "Andras",
				"Andrea", "Andromeda", "Aneida", "Anella", "Anemone", "Anezka", "Angela", "Angelica", "Angeline", "Angelique", "Angeni", "Ani",
				"Anica", "Anieli", "Anisa", "Anita", "Anke", "Ann", "Anna", "Annabel", "Annabelle", "Annamarie", "Anne", "Annette", "Annikka",
				"Annora", "Anorah", "Anoush", "Ansreana", "Anteia", "Anthea", "Antje", "Antoinette", "Antonia", "Aolani", "Apara", "Apirka",
				"Apolline", "Apolloina", "Aponi", "April", "Aprille", "Aprille", "Aqua", "Aquene", "Ara", "Arabella", "Arabelle", "Araceli",
				"Araminta", "Araxie", "Arcadia", "Ardath", "Ardelia", "Arden", "Ardis", "Ardith", "Areiela", "Arella", "Aretha", "Aretina",
				"Ariadne", "Ariana", "Aricia", "Ariel", "Ariene", "Arista", "Arlene", "Arlinda", "Armina", "Arminda", "Artemisia", "Aruna",
				"Arziki", "Asaria", "Asenka", "Ash", "Asha", "Ashlan", "Ashleigh", "Ashley", "Asia", "Asisa", "Aslinda", "Aspasia", "Asta",
				"Aster", "Astera", "Astra", "Astrea", "Astrid", "Atalanta", "Atara", "Atenne", "Ateri", "Athalia", "Athena", "Athla", "Atifa",
				"Atta", "Aubrey", "Auda", "Audny", "Audrey", "Audrianna", "Audun", "Augustina", "Aura", "Aure", "Aurelia", "Aurilia", "Aurina",
				"Aurkene", "Aurora", "Autumn", "Ava", "Avana", "Avasa", "Avella", "Avena", "Avie", "Avis", "Aviva", "Axella", "Aya", "Ayaluna",
				"Ayame", "Ayana", "Ayasha", "Aydee", "Ayela", "Ayiana", "Ayila", "Ayisha", "Ayita", "Ayla", "Aynora", "Ayuna", "Azaleah",
				"Azalia", "Azarael", "Azera", "Azha", "Azilea", "Azina", "Azize", "Azora", "Azura", "Babette", "Bacia", "Bacia", "Baka", "Baka",
				"Bakarne", "Balayna", "Balea", "Balia", "Bambi", "Banan", "Banella", "Bara", "Barbara", "Barika", "Basha", "Basha", "Basia",
				"Basimah", "Batakah", "Bathsheba", "Batya", "Bay", "Bayana", "Bayo", "Bayta", "Bea", "Beatrice", "Beatrix", "Beauina", "Becca",
				"Becky", "Bedelia", "Bel", "Belana", "Belina", "Belinda", "Belita", "Bellanca", "Belle", "Belora", "Bente", "Beradine", "Berilla",
				"Berit", "Bernadette", "Bernice", "Beryl", "Bess", "Bessine", "Beta", "Beth", "Bethana", "Bethany", "Betony", "Betty", "Beulah",
				"Beverly", "Bevin", "Bian", "Bianca", "Billie", "Bina", "Bindy", "Binti", "Birdie", "Birkita", "Bixenta", "Blanche", "Blanda",
				"Blenda", "Bliss", "Bly", "Blythe", "Bo", "Bohdana", "Bonamy", "Bonita", "Bonnie", "Bonny", "Borgny", "Braina", "Brandi",
				"Brandy", "Bren", "Brenda", "Brenna", "Bretta", "Bridget", "Bridget", "Brie", "Brier", "Brietta", "Brigit", "Brigitte", "Brina",
				"Brina", "Briona", "Briony", "Brites", "Britta", "Brittany", "Bronwyn", "Brooke", "Brynn", "Bucia", "Cadence", "Caimile",
				"Caitlin", "Caitrin", "Cala", "Calandia", "Calandra", "Calendonia", "Caley", "Calida", "Calista", "Calla", "Callan", "Callia",
				"Callidora", "Callie", "Caltha", "Calypso", "Cam", "Camelia", "Camilia", "Camille", "Canace", "Candace", "Candida", "Candide",
				"Candra", "Cantara", "Caoimhe", "Capri", "Caprice", "Cara", "Caradoc", "Caresse", "Cari", "Carina", "Carine", "Carissa", "Carita",
				"Carla", "Carleen", "Carlen", "Carling", "Carlota", "Carly", "Carma", "Carmel", "Carmelina", "Carmen", "Carna", "Carnelian",
				"Carol", "Carolina", "Caroline", "Carolyn", "Caron", "Carrie", "Caryn", "Casey", "Casilda", "Cassandra", "Cassia", "Casta",
				"Castalia", "Catalina", "Catava", "Caterina", "Catherine", "Cathleen", "Cathy", "Catriona", "Cayla", "Ceara", "Cecania", "Cecilia",
				"Celandine", "Celeste", "Celia", "Celina", "Celina", "Cellia", "Cerelia", "Chaitra", "Chanah", "Chanda", "Chandi", "Chandra",
				"Chane", "Chanel", "Channa", "Chantal", "Charis", "Charissa", "Charity", "Charlotte", "Charmaine", "Chastity", "Chava", "Chaviva",
				"Chay", "Chaya", "Chelsea", "Chenoa", "Cherica", "Cherice", "Cherie", "Cheryl", "Chesna", "Chiara", "Chika", "Chilali", "Chimlis",
				"Chipo", "Chloe", "Chloris", "Cho", "Christa", "Christable", "Christina", "Christine", "Christy", "Chyou", "Cia", "Ciannait",
				"Ciar", "Cicely", "Cindy", "Claire", "Clara", "Clarinda", "Clarissa", "Claudette", "Claudia", "Claudine", "Clementina", "Clementine",
				"Cliantha", "Clorinda", "Clorinda", "Clover", "Cochiti", "Coleene", "Colette", "Connie", "Constance", "Constanza", "Consuela",
				"Cora", "Coralie", "Corazon", "Corbey", "Cordeali", "Coretta", "Cori", "Corinna", "Coris", "Corliss", "Corrine", "Cortney",
				"Crescent", "Cressida", "Crystal", "Cybele", "Cybil", "Cynthia", "Cyprien", "Cyrene", "Cyrilla", "Cytheria", "Dabria", "Dacey",
				"Dacia", "Dacie", "Dacio", "Dae", "Dagmar", "Dagna", "Dai", "Daily", "Daisel", "Daisy", "Dakota", "Dale", "Dalila", "Dalilia",
				"Damara", "Damitri", "Dana", "Danett", "Dania", "Daniella", "Danyelle", "Daphene", "Daphne", "Daphnie", "Dar", "Dara", "Daralis",
				"Darby", "Daria", "Darla", "Darlene", "Dasha", "Dasha", "Davene", "Davine", "Davita", "Dawn", "Daya", "Dayna", "Deana", "Deana",
				"Deandra", "Deb", "Debra", "Dede", "Dee", "Deedee", "Deianira", "Deiene", "Deirdre", "Delana", "Delaney", "Delbin", "Delia",
				"Delicia", "Delilia", "Della", "Delphina", "Dembe", "Demi", "Demitria", "Dena", "Denby", "Denice", "Deva", "Devaki", "Deval",
				"Devi", "Devin", "Devnet", "Devon", "Diamanta", "Diane", "Dianthe", "Diedre", "Diella", "Dillian", "Dilly", "Dilys", "Dinah",
				"Dionne", "Disa", "Dita", "Diti", "Dixie", "Dodie", "Dolores", "Dominique", "Dona", "Donata", "Donielle", "Donner", "Dooriya",
				"Dophina", "Dora", "Doreen", "Dorinda", "Doris", "Dorithy", "Dory", "Drew", "Drina", "Drucilla", "Dulcie", "Dulcinea", "Dusty",
				"Dyan", "Dyani", "Dymphna", "Dyna", "Eartha", "Easter", "Ebony", "Echo", "Edana", "Edie", "Edith", "Edlyn", "Edna", "Edolie",
				"Edria", "Edwina", "Efia", "Eileen", "Eirene", "Elaine", "Elana", "Eleora", "Elianor", "Elina", "Elina", "Elisa", "Elise",
				"Eliska", "Elissa", "Elita", "Eliza", "Elizabeeth", "Elke", "Ella", "Elle", "Ellen", "Elly", "Elodie", "Eloise", "Elsa", "Elsie",
				"Elynor", "Elyse", "Elysia", "Ema", "Emajane", "Emalia", "Ember", "Emelie", "Emelyne", "Emily", "Emma", "Endora", "Engracia",
				"Enid", "Enola", "Enye", "Erasma", "Erianthe", "Erica", "Erin", "Erlina", "Erwand", "Eskarne", "Esmerelda", "Esperanza", "Esta",
				"Estelle", "Esther", "Estu", "Etain", "Etaina", "Etaina", "Etanthe", "Etta", "Eudocia", "Eugenia", "Eulalia", "Eustacia",
				"Eva", "Evacsa", "Evadine", "Evadne", "Evangeline", "Evanthe", "Eve", "Evelyn", "Evita", "Evonne", "Eyota", "Fabienne", "Faifuza",
				"Fainche", "Faith", "Faizah", "Fallon", "Fantine", "Farha", "Farima", "Farrah", "Fatin", "Fawne", "Fay", "Faye", "Fayina",
				"Fayme", "Felcia", "Felicite", "Felicity", "Femi", "Feridwyn", "Fern", "Feronia", "Filinda", "Fina", "Finola", "Fiona", "Fiorenza",
				"Flavia", "Fleta", "Flora", "Florence", "Frances", "Francesca", "Francine", "Francisca", "Freda", "Frederica", "Freya", "Frida",
				"Frieda", "Fuscienne", "Gabriella", "Gabrielle", "Gaia", "Gail", "Galatea", "Gali", "Galina", "Galya", "Gana", "Ganesa", "Gauri",
				"Gaye", "Gayle", "Gelasia", "Gemma", "Genevieve", "Geogia", "Georgeanne", "Georgetta", "Georgette", "Georgiana", "Geradline",
				"Geraldine", "Gerda", "Gerri", "Gertrude", "Geva", "Ghislaine", "Giacinta", "Gianina", "Gigi", "Gilana", "Gilda", "Gilen",
				"Gillian", "Gin", "Gina", "Ginger", "Giselle", "Gitana", "Githa", "Gizane", "Gleda", "Glenna", "Glennys", "Golda", "Goldie",
				"Gotzone", "Grace", "Gracie", "Grainne", "Grazia", "Grear", "Greta", "Gretchen", "Grette", "Gwen", "Gwendolyn", "Gweneth",
				"Gwynne", "Gytha", "Hadara", "Hadassa", "Hadiya", "Haidee", "Hailey", "Haimi", "Haldis", "Hale", "Haley", "Hali", "Hali",
				"Halima", "Halle", "Hallie", "Hana", "Hanan", "Hannah", "Hanne", "Harmoni", "Harriet", "Hasna", "Hava", "Haya", "Haylee",
				"Hazel", "Hea", "Heather", "Hei", "Heidi", "Heldegarde", "Helen", "Helena", "Helene", "Helki", "Henka", "Henrietta", "Hesper",
				"Hester", "Hilary", "Hilda", "Hinda", "Hisa", "Holly", "Hope", "Hoshi", "Hyacinth", "Hye", "Hypatia", "Ianthe", "Ida", "Idola",
				"Idonia", "Ilene", "Ilona", "Iman", "Imogene", "India", "Indira", "Indra", "Ines", "Inez", "Inga", "Ingrid", "Iolana", "Iolanthe",
				"Iona", "Iratze", "Irena", "Irene", "Iris", "Irma", "Isabeau", "Isabel", "Isabella", "Isadora", "Isaura", "Isis", "Isleta",
				"Isobel", "Isoke", "Istas", "Ivana", "Ivory", "Ivy", "Jacelyn", "Jacinda", "Jacinthe", "Jada", "Jael", "Jaen", "Jaimie", "Jaione",
				"Jakinda", "Jala", "Jamie", "Jamila", "Jamilah", "Jan", "Jana", "Jane", "Janelle", "Janet", "Janice", "Janis", "Janna", "Jannelle",
				"Jardena", "Jarvia", "Jarvinia", "Jasmine", "Jaya", "Jayne", "Jean", "Jean", "Jeanette", "Jeanine", "Jelena", "Jena", "Jenay",
				"Jendayi", "Jendyose", "Jenica", "Jennettia", "Jennifer", "Jensine", "Jerrilyn", "Jessica", "Jewel", "Jezebel", "Jihan", "Jillian",
				"Jin", "Jina", "Jinny", "Jinx", "Joakima", "Joan", "Joanne", "Jobey", "Jobihna", "Jocasa", "Jocelyn", "Jodi", "Jody", "Joelle",
				"Joelliane", "Johanna", "Joia", "Jolan", "Jolanta", "Jolene", "Jolie", "Joline", "Jonina", "Jora", "Jordane", "Josephine",
				"Josie", "Jotha", "Joy", "Joyce", "Joye", "Juanita", "Judith", "Juditha", "Julia", "Juliana", "Juliane", "Julie", "Julietta",
				"Julinka", "Jumoke", "Jun", "June", "Justine", "Kaatje", "Kachine", "Kaclyn", "Kaede", "Kaethe", "Kai", "Kaia", "Kaie", "Kaili",
				"Kaimi", "Kairos", "Kaiya", "Kakra", "Kala", "Kalama", "Kalanit", "Kalare", "Kalea", "Kali", "Kalika", "Kalila", "Kalinda",
				"Kalle", "Kalli", "Kalonice", "Kalyca", "Kama", "Kamala", "Kamali", "Kamaria", "Kambo", "Kameko", "Kamilah", "Kamilia", "Kanda",
				"Kane", "Kanene", "Kanika", "Kantha", "Kanya", "Kapera", "Kara", "Karan", "Karayan", "Karel", "Karen", "Karida", "Karimah",
				"Karisa", "Karka", "Karla", "Karlenne", "Karli", "Karlyn", "Karmina", "Karol", "Karylin", "Karyn", "Kasa", "Kasen", "Kasia",
				"Kasinda", "Kassia", "Kate", "Katherine", "Kathleen", "Katja", "Katoka", "Katrien", "Katrina", "Kaula", "Kaveri", "Kavindra",
				"Kay", "Kaya", "Kaye", "Kayla", "Kaysa", "Kazia", "Keara", "Keelin", "Keely", "Kefira", "Kehinde", "Kei", "Keiko", "Keisha",
				"Kelda", "Kelia", "Kelley", "Kelli", "Kellie", "Kelly", "Kelsey", "Kendra", "Kennis", "Kenyangi", "Kepa", "Kerani", "Kerensa",
				"Kerstan", "Kesare", "Kesi", "Kesia", "Kessie", "Keturah", "Ketzia", "Khalida", "Kichi", "Kiele", "Kim", "Kimberly", "Kimmie",
				"Kimmy", "Kineta", "Kiona", "Kira", "Kiran", "Kirby", "Kirima", "Kirsten", "Kirti", "Kisa", "Kiska", "Kismet", "Kissa", "Kita",
				"Kohana", "Kolina", "Koren", "Koressa", "Kristen", "Kyly", "Kyna", "Kynthia", "Kyoko", "Lacey", "Lacie", "Laila", "Lailie",
				"Lakeisha", "Lala", "Lalasa", "Lan", "Lana", "Landra", "Lane", "Lani", "Lara", "Laraine", "Laralee", "Lari", "Larissa", "Lark",
				"Latika", "Latonia", "Laura", "Laurana", "Laurel", "Laurie", "Laurinda", "Lauryn", "Laveda", "Lavern", "Laverne", "Lavinia",
				"Lea", "Leah", "Leah", "Leala", "Leandra", "Leba", "Ledah", "Lee", "Leigh", "Leiko", "Leila", "Leilana", "Lena", "Lene", "Lenor",
				"Lenora", "Lenore", "Leona", "Leora", "Leslie", "Letha", "Letitia", "Levana", "Lexine", "Lia", "Liadan", "Lian", "Liana",
				"Liane", "Libby", "Lien", "Lila", "Lilith", "Lillian", "Lillie", "Lily", "Limber", "Lina", "Linda", "Lindsay", "Lindsey",
				"Linette", "Linnae", "Linnea", "Lisa", "Lisette", "Litsa", "Liv", "Liza", "Lois", "Lokelani", "Lola", "Loni", "Lora", "Lore",
				"Lorelei", "Lorelle", "Loretta", "Lori", "Lorraine", "Lotus", "Louise", "Lucille", "Lucine", "Lucretia", "Lucy", "Ludia",
				"Luela", "Luisa", "Lukene", "Lukina", "Lulu", "Luna", "Lydia", "Lynda", "Lynelle", "Lynn", "Lynnda", "Lynnette", "Lyris",
				"Lysel", "Lysnadra", "Mabel", "Macaria", "Machi", "Maddy", "Madelaine", "Madelina", "Madeline", "Madelon", "Madelyn", "Mady",
				"Mae", "Magan", "Magara", "Magdalen", "Magdalena", "Magdaline", "Magena", "Magenta", "Maggie", "Mahala", "Mahalia", "Mai",
				"Maia", "Maida", "Maisie", "Maitane", "Maizah", "Maj", "Malaya", "Malila", "Malina", "Malinda", "Malka", "Mallory", "Malu",
				"Mamie", "Manda", "Mandara", "Mandisa", "Mandy", "Mangena", "Manon", "Mansi", "Manya", "Mara", "Marcella", "Marcia", "Marcy",
				"Maren", "Margaret", "Margaret", "Margarita", "Margo", "Margot", "Marguirte", "Maria", "Mariah", "Mariam", "Mariama", "Marian",
				"Mariana", "Marianna", "Marianne", "Maribel", "Marie", "Mariel", "Marietta", "Marily", "Marilyn", "Marina", "Maris", "Marisa",
				"Marisha", "Marissa", "Marjani", "Marjeta", "Marjorie", "Marlene", "Marlo", "Marmara", "Marnie", "Marnina", "Marsha", "Marta",
				"Martha", "Marti", "Martina", "Mary", "Maryann", "Marybeth", "Marylou", "Marzia", "Matana", "Mathea", "Matilda", "Matrika",
				"Maud", "Maura", "Maureen", "Maurita", "Mavis", "Maxine", "May", "Maya", "Meara", "Meara", "Meda", "Medea", "Meg", "Megan",
				"Megara", "Meghan", "Mei", "Meira", "Mela", "Melanie", "Melantha", "Melba", "Melia", "Melian", "Melina", "Melinda", "Melisenda",
				"Melissa", "Mellinio", "Melodie", "Melody", "Melosa", "Melva", "Mercedes", "Meredith", "Merele", "Mesha", "Meta", "Mia", "Miakoda",
				"Michaela", "Michele", "Michelle", "Midori", "Migina", "Mignon", "Mika", "Millicent", "Millie", "Min", "Mina", "Minda", "Mindel",
				"Mindy", "Minerva", "Minka", "Minna", "Minnie", "Mira", "Miranda", "Mirem", "Miremba", "Mireya", "Miriam", "Mirielle", "Missy",
				"Misty", "Mitena", "Mitexi", "Mitzi", "Moira", "Mollie", "Molly", "Mona", "Monique", "Moon", "Morena", "Morgan", "Morgana",
				"Morgance", "Moria", "Moriah", "Muriel", "Myra", "Nada", "Nadia", "Nadine", "Nadya", "Naia", "Nailah", "Naimah", "Nalini",
				"Namazzi", "Nami", "Nan", "Nana", "Nancy", "Nanette", "Nantale", "Naomi", "Napea", "Nara", "Narda", "Narmada", "Nasiche",
				"Nastassia", "Natalie", "Natane", "Natasha", "Natesa", "Naysa", "Nazirqah", "Neala", "Neci", "Nediva", "Neely", "Nekane",
				"Nell", "Neola", "Neoma", "Neona", "Neria", "Nerine", "Nerissa", "Netti", "Neva", "Nevada", "Neysa", "Nicia", "Nicola", "Nicole",
				"Nicolette", "Nika", "Nikki", "Nimah", "Nina", "Niobe", "Niola", "Nira", "Nirvelli", "Nissa", "Nita", "Nitara", "Nixie", "Noel",
				"Noelani", "Noella", "Nolita", "Nona", "Nona", "Nora", "Norah", "Noreen", "Nori", "Noriko", "Norma", "Nydia", "Nyrna", "Nyssa",
				"Obelia", "Octavia", "Odelia", "Odelia", "Odera", "Odessa", "Odetta", "Odette", "Odile", "Ohanna", "Okelani", "Olathe", "Olayinka",
				"Olesia", "Olga", "Oliana", "Olinda", "Olivette", "Olivia", "Ona", "Onida", "Opal", "Ophelia", "Oralie", "Orane", "Orenda",
				"Oriana", "Orianna", "Oriel", "Oriole", "Orlantha", "Ornidaa", "Paige", "Pakuna", "Palmiera", "Paloma", "Pamela", "Pandita",
				"Pandora", "Panthea", "Pantzike", "Panya", "Panyin", "Pascale", "Patia", "Patience", "Patricia", "Patsy", "Paula", "Paulette",
				"Pauline", "Pavla", "Pazia", "Pearl", "Peg", "Peggy", "Pelagia", "Pemba", "Penda", "Penelope", "Peninna", "Penny", "Penthea",
				"Peony", "Perdita", "Perouze", "Persis", "Petra", "Phaedra", "Phedra", "Philomena", "Phoebe", "Phylis", "Phyllis", "Pia",
				"Pier", "Pila", "Piper", "Polly", "Poloma", "Porche", "Portia", "Priscilla", "Prudence", "Prudy", "Pyrena", "Pythia", "Qamra",
				"Queena", "Quella", "Quenby", "Quintina", "Quiterie", "Rachel", "Radella", "Radinka", "Rae", "Rai", "Raizel", "Ramla", "Ramona",
				"Ramya", "Randie", "Rane", "Ranee", "Rani", "Raquel", "Rashida", "Rasine", "Ratri", "Raven", "Rawnie", "Rayna", "Raynell",
				"Raziya", "Reba", "Rebecca", "Regan", "Regina", "Reidun", "Remy", "Rena", "Renata", "Rene", "Renee", "Rhea", "Rhiamon", "Rhianne",
				"Rhiannon", "Rhoda", "Rhodanthe", "Rhonda", "Rhonna", "Rhyssa", "Ria", "Riane", "Rica", "Rihana", "Rikki", "Rio", "Risa",
				"Rita", "Riva", "Roanna", "Roberta", "Robin", "Robyn", "Rochelle", "Rohanna", "Rona", "Rorie", "Rosa", "Rosalind", "Rosalinda",
				"Rosalinde", "Rosaline", "Rosanne", "Rose", "Roseanne", "Rosemarie", "Rosemary", "Rowena", "Roxana", "Roxanne", "Ruby", "Rumer",
				"Ruth", "Ruthann", "Ryann", "Ryanne", "Ryba", "Ryssa", "Saba", "Sabina", "Sabiny", "Sabirah", "Sabra", "Sabrina", "Sacha",
				"Sachi", "Sade", "Sadira", "Saffi", "Safiya", "Sagara", "Saidah", "Sakari", "Sakinah", "Sakti", "Sakura", "Salihah", "Salimah",
				"Salina", "Sally", "Salome", "Samantha", "Samara", "Samirah", "Sancia", "Sandia", "Sandra", "Sandrine", "Sandya", "Sara",
				"Sarah", "Sarai", "Saraid", "Saree", "Sarena", "Sari", "Sarisha", "Sasha", "Sashenka", "Satinka", "Savanna", "Saxon", "Scotia",
				"Searlait", "Season", "Sebasten", "Seema", "Sela", "Selena", "Selina", "Selma", "Semele", "Senta", "Serafina", "Serilda",
				"Sesha", "Shaine", "Shakira", "Shako", "Shammara", "Shana", "Shanata", "Shandra", "Shandy", "Shani", "Shanley", "Shanna",
				"Shannon", "Shantay", "Shantha", "Sharman", "Sharon", "Sharri", "Shashi", "Shawn", "Shayndel", "Sheba", "Sheena", "Sheila",
				"Shela", "Shelby", "Shelley", "Shelly", "Sherri", "Shika", "Shin", "Shina", "Shira", "Shirley", "Shobi", "Shoshana", "Sibley",
				"Sibongile", "Sibyl", "Sidonia", "Sidra", "Sierra", "Sigourney", "Siham", "Sileas", "Silva", "Silvia", "Simba", "Simone",
				"Sine", "Sinead", "Siobhan", "Siran", "Sirena", "Siroun", "Sitara", "Sitembile", "Siv", "Sive", "Skyler", "Sofi", "Solana",
				"Solange", "Soledad", "Solita", "Sondra", "Sonia", "Sonja", "Sonya", "Sophia", "Sophie", "Sophronia", "Spica", "Stacey", "Stacia",
				"Stacy", "Stefania", "Stella", "Stephani", "Stephanie", "Ster", "Stesha", "Stockard", "Storm", "Sukatai", "Suki", "Sumi",
				"Summer", "Sun", "Susan", "Susanna", "Suzanne", "Svetlana", "Sybil", "Sydelle", "Sydney", "Syeira", "Sylvia", "Syna", "Synia",
				"Tabitha", "Taci", "Tacita", "Tadi", "Taffy", "Tahirah", "Tai", "Taima", "Tainn", "Taipa", "Taite", "Taka", "Takara", "Takiyah",
				"Takoda", "Talasi", "Tale", "Talia", "Talia", "Talitha", "Tallulah", "Tam", "Tama", "Tamara", "Tamary", "Tamma", "Tammy",
				"Tanaka", "Tani", "Tansy", "Tanya", "Tao", "Tara", "Tate", "Tatyana", "Tawnie", "Tawny", "Tayce", "Taylor", "Teague", "Tehya",
				"Tekla", "Temina", "Terentia", "Terese", "Terrilyn", "Tertia", "Teryn", "Tesia", "Tess", "Tessa", "Thadea", "Thais", "Thalassa",
				"Thalia", "Than", "Thana", "Thara", "Thea", "Thekla", "Thelma", "Theodosia", "Theone", "Thera", "Thirza", "Thora", "Thyra",
				"Tia", "Tiara", "Tienette", "Tierney", "Tierra", "Tiffany", "Tilda", "Timandra", "Tina", "Tiponya", "Tirza", "Tivona", "Tobey",
				"Tola", "Tora", "Tori", "Tory", "Tosia", "Tove", "Tracey", "Tracy", "Treasa", "Tresa", "Treva", "Trianon", "Tricia", "Trilby",
				"Trina", "Trind", "Trish", "Trisha", "Trudy", "Tryne", "Tryphena", "Tyne", "Ula", "Ulani", "Ultima", "Uma", "Una", "Undine",
				"Undine", "Urania", "Uriana", "Ursula", "Uta", "Vala", "Valentina", "Valeria", "Valerie", "Valeska", "Valonia", "Valora",
				"Vanda", "Vanessa", "Vanora", "Vanya", "Vashti", "Veda", "Velika", "Velma", "Venesssa", "Vera", "Verena", "Verity", "Veronica",
				"Vesta", "Vevila", "Victoria", "Vidonia", "Violet", "Violet", "Violetta", "Virginia", "Viridis", "Viveka", "Vivian", "Voleta",
				"Vrinda", "Wakanda", "Wanda", "Waneta", "Wendy", "Whilhelmina", "Whitney", "Wijdan", "Willow", "Wilma", "Wilona", "Winda",
				"Winema", "Winifred", "Winna", "Winona", "Wynee", "Wynn", "Wynona", "Xanthe", "Xaveria", "Xaviera", "Xena", "Xenia", "Ximena",
				"Xylia", "Xylona", "Yachne", "Yanice", "Yarmilla", "Yasmeen", "Yasmin", "Yelinda", "Yenene", "Yesmina", "Yetta", "Yeva", "Yokiko",
				"Yolanda", "Yolie", "Yonina", "Yovela", "Yvella", "Yvette", "Yvonne", "Zada", "Zahara", "Zahirah", "Zahra", "Zakia", "Zalea",
				"Zalika", "Zaltana", "Zandra", "Zara", "Zarah", "Zaza", "Zehava", "Zelda", "Zelenka", "Zelia", "Zella", "Zena", "Zenaide",
				"Zenia", "Zerlinda", "Zeva", "Zevida", "Zia", "Ziazan", "Zigana", "Zila", "Zina", "Zinnia", "Zita", "Zoe", "Zola", "Zona",
				"Zora", "Zorah", "Zorda", "Zosia", "Zuleika", "Zulema", "Zuza", "Zuzanny",
			}),

			#endregion

			#region Male

			new("Male", new[]
			{
				"Aaron", "Aasin", "Abbott", "Abdel", "Abdiel", "Abel", "Abijah", "Abner", "Abraham", "Abran", "Ace", "Achilles", "Ackerley",
				"Adair", "Adam", "Addison", "Adeben", "Adem", "Adiran", "Adlai", "Adler", "Adley", "Admon", "Adolph", "Adon", "Adonis", "Adrian",
				"Adriel", "Aeneas", "Agustin", "Ahearn", "Ahmik", "Ahren", "Aidan", "Aiken", "Aimery", "Aitan", "Ajayi", "Akando", "Akbaar",
				"Akello", "Akil", "Akshay", "Alan", "Aland", "Alano", "Alaric", "Alastair", "Alben", "Albert", "Alcander", "Alcott", "Alden",
				"Alder", "Aldrick", "Alec", "Alek", "Aleksy", "Aleron", "Aleser", "Alex", "Alexander", "Alfred", "Alger", "Alim", "Alistair",
				"Allaard", "Allan", "Allard", "Allen", "Alonzo", "Alphonse", "Alphonso", "Alston", "Altair", "Alton", "Alvin", "Amadeo", "Amadi",
				"Amado", "Ambrose", "Amiel", "Ammon", "Amos", "Amsden", "Anders", "Andre", "Andreus", "Andrew", "Andrey", "Andries", "Angelo",
				"Angus", "Anker", "Anoki", "Ansel", "Anselme", "Ansley", "Anson", "Anthony", "Antonio", "Anwar", "Archer", "Archibald", "Ardon",
				"Aren", "Ares", "Argus", "Ari", "Aricin", "Arion", "Aristo", "Aristotle", "Arkin", "Arlen", "Arley", "Arlin", "Arlo", "Arman",
				"Armen", "Armon", "Armstrong", "Arne", "Arnold", "Arnon", "Aron", "Arpiar", "Arsen", "Arsenio", "Arthur", "Ashby", "Asher",
				"Ashford", "Ashlin", "Ashon", "Ashur", "Athan", "Atheron", "Atman", "Audric", "Audun", "Augustin", "Augustus", "Aurek", "Austin",
				"Averill", "Avery", "Axel", "Bae", "Bailey", "Baingana", "Bakari", "Balbo", "Balder", "Baldwin", "Bale", "Balendin", "Bali",
				"Balin", "Balint", "Bancroft", "Bandele", "Bane", "Banning", "Baran", "Barclay", "Barden", "Bardo", "Bardon", "Barnabas",
				"Barnaby", "Barnett", "Barney", "Baron", "Barrett", "Barry", "Barse", "Bart", "Barth", "Bartholomew", "Barton", "Basil", "Bastiaan",
				"Baul", "Bavol", "Baxter", "Bay", "Bayani", "Bayard", "Baylor", "Bazyli", "Beacan", "Beagan", "Beaman", "Beau", "Beaumont",
				"Beauregard", "Beck", "Beldon", "Belen", "Bem", "Beman", "Ben", "Benedict", "Benen", "Benjamin", "Bennett", "Benson", "Bent",
				"Bentley", "Benton", "Berenger", "Bergren", "Berk", "Berkeley", "Bernard", "Bersh", "Bert", "Berthold", "Berton", "Bertram",
				"Beval", "Bevan", "Bialy", "Bilal", "Bishop", "Bitalo", "Bjorn", "Blade", "Blaine", "Blair", "Blaise", "Blake", "Blaz", "Blorn",
				"Bo", "Boden", "Bogart", "Bohdan", "Bolton", "Bond", "Booker", "Boone", "Borden", "Boris", "Botan", "Bowie", "Bowman", "Boyce",
				"Boyd", "Boyden", "Brad", "Braden", "Bradford", "Bradney", "Brady", "Bram", "Bran", "Brand", "Brandeis", "Brandon", "Brant",
				"Braxton", "Bray", "Braz", "Brazil", "Bren", "Brencis", "Brendan", "Brendon", "Brennan", "Brent", "Brentan", "Bret", "Brett",
				"Brewster", "Briac", "Brian", "Briand", "Brice", "Brieg", "Brinley", "Brishen", "Brock", "Broderick", "Brodny", "Brody", "Bronson",
				"Bront", "Bruce", "Bruno", "Brutus", "Bryan", "Bryant", "Bryce", "Bryson", "Buck", "Bud", "Budo", "Burgess", "Burhan", "Burian",
				"Burke", "Burl", "Burr", "Burton", "Byran", "Byron", "Cadeo", "Cador", "Caedmon", "Cailan", "Cain", "Caine", "Calder", "Caldwell",
				"Caleb", "Calvin", "Cam", "Camden", "Cameron", "Candan", "Canton", "Canute", "Carden", "Carey", "Carl", "Carlin", "Carlo",
				"Carlos", "Carlton", "Carr", "Carrick", "Carrocio", "Carroll", "Carson", "Carson", "Carter", "Carver", "Casey", "Casper",
				"Cassidy", "Cassius", "Castel", "Cato", "Caton", "Cavan", "Ceasar", "Cecil", "Cedric", "Cemal", "Chad", "Chadwick", "Chaim",
				"Chal", "Chale", "Chalmers", "Chander", "Chandler", "Chane", "Chaney", "Channing", "Chapin", "Chapman", "Charles", "Charlton",
				"Chase", "Chatha", "Chauncy", "Chayton", "Chen", "Cheney", "Chester", "Chet", "Chevalier", "Chike", "Chin", "Christian", "Christoph",
				"Christopher", "Christos", "Chuck", "Ciceron", "Ciro", "Clarence", "Clark", "Claude", "Clay", "Clayton", "Clement", "Cleveland",
				"Clifford", "Clifton", "Clint", "Clinton", "Clive", "Clyde", "Cody", "Colby", "Cole", "Coleman", "Colin", "Collin", "Colon",
				"Colton", "Coman", "Condon", "Connor", "Conrad", "Conway", "Corbett", "Corbin", "Corcoran", "Cordell", "Corey", "Cornelius",
				"Cort", "Coty", "Courtland", "Craig", "Crandall", "Creighton", "Crispin", "Crosby", "Cullen", "Cullin", "Culver", "Curran",
				"Curtis", "Cynric", "Cyrano", "Cyril", "Cyrus", "Dag", "Dagan", "Dakarai", "Dakota", "Dale", "Dallin", "Dalton", "Daly", "Damek",
				"Damen", "Damian", "Damien", "Damion", "Damon", "Dana", "Dane", "Daniel", "Danior", "Dannik", "Dante", "Daren", "Darien",
				"Dario", "Darnell", "Darrel", "Darrell", "Darren", "Daryl", "David", "Davin", "Davis", "Deacon", "Dean", "Decker", "Delaney",
				"Delano", "Delbert", "Dellan", "Delmore", "Delsin", "Deman", "Dempsey", "Dempster", "Denby", "Dennis", "Dennys", "Denton",
				"Denver", "Der", "Derek", "Derrick", "Derry", "Deverell", "Devin", "Devlin", "Dewey", "Diederik", "Diego", "Dieter", "Dillon",
				"Dimitri", "Dirk", "Dobry", "Dominic", "Dominick", "Donald", "Donatien", "Donato", "Donnelley", "Donnelly", "Donovan", "Doron",
				"Dougie", "Douglas", "Douglass", "Dov", "Doyle", "Drake", "Drew", "Duane", "Dugan", "Duglas", "Duncan", "Dunstan", "Durand",
				"Durriken", "Dusan", "Dustin", "Dutch", "Dwayne", "Dwight", "Dyami", "Dyastro", "Dylan", "Dymas", "Eamon", "Earl", "Earle",
				"Eaton", "Edan", "Edgan", "Edgar", "Edison", "Edmund", "Edrin", "Edward", "Edwin", "Edwin", "Egan", "Einar", "Elad", "Elden",
				"Eldroth", "Elek", "Eli", "Elias", "Elijah", "Elkan", "Ellery", "Elliot", "Elliott", "Ellis", "Ellsworth", "Elmer", "Elmo",
				"Elston", "Elton", "Elwood", "Emanuel", "Emil", "Emilio", "Emmett", "Emo", "Enoch", "Enrico", "Enrique", "Ephraim", "Erek",
				"Eric", "Erik", "Ernest", "Erol", "Errol", "Erskine", "Erwin", "Eryx", "Essien", "Esteban", "Ethan", "Eugene", "Evan", "Evander",
				"Everett", "Evzen", "Ezekial", "Ezra", "Fabio", "Fairfax", "Farley", "Farrell", "Faxon", "Felix", "Felix", "Fenn", "Fenton",
				"Fergus", "Ferran", "Ferris", "Fielding", "Filbert", "Filmore", "Finlay", "Finley", "Finn", "Finnigan", "Fisk", "Fitzgerald",
				"Fletcher", "Flindo", "Flint", "Floyd", "Flynn", "Forbes", "Forrest", "Forsythe", "Foster", "Foster", "Francis", "Franek",
				"Frank", "Franklin", "Frasier", "Frazer", "Frazier", "Fred", "Frederick", "Fremont", "Fritz", "Fuller", "Fulton", "Gabe",
				"Gabriel", "Gage", "Galen", "Galeno", "Galvin", "Gamble", "Gannon", "Gareth", "Garfield", "Gargan", "Garner", "Garrett", "Garrick",
				"Garridan", "Garrison", "Garritt", "Garth", "Garvin", "Gary", "Gaspar", "Gaston", "Gavin", "Gavrie", "Gaylord", "Gaynor",
				"Geoff", "Geoffrey", "Geoffry", "George", "Gerard", "Gerik", "Germain", "Gerry", "Gideon", "Gilberto", "Giles", "Ginton",
				"Givon", "Glen", "Glenn", "Glenno", "Godfrey", "Gordon", "Gordy", "Gorman", "Grady", "Graham", "Gram", "Granger", "Grant",
				"Granville", "Grayson", "Greg", "Greger", "Gregor", "Gregory", "Gresham", "Griffen", "Griffith", "Guilhem", "Gunnar", "Gunther",
				"Gus", "Gustave", "Guthrie", "Guy", "Hackett", "Hadden", "Hadi", "Hadley", "Hadrian", "Hagan", "Hal", "Halden", "Hale", "Halian",
				"Halsey", "Hamilton", "Hamlin", "Hank", "Hans", "Harden", "Hardy", "Harith", "Harlan", "Harman", "Harold", "Harper", "Harrison",
				"Harry", "Hart", "Hartley", "Harvey", "Hassan", "Hastin", "Hastings", "Hayden", "Hayes", "Haynes", "Heath", "Hector", "Helaku",
				"Henning", "Henry", "Herbert", "Herman", "Herschel", "Hilliard", "Hilton", "Hiroshi", "Hobart", "Hogan", "Holden", "Holt",
				"Homes", "Horace", "Horton", "Houston", "Howard", "Howrence", "Hoyt", "Hugh", "Hugo", "Humphrey", "Hunter", "Huntley", "Hyman",
				"Iain", "Ian", "Ilias", "Ingmar", "Ingram", "Ira", "Irvin", "Irving", "Irwin", "Isaac", "Isaiah", "Israel", "Itzak", "Ivan",
				"Ivar", "Jabari", "Jabir", "Jack", "Jacob", "Jacobe", "Jacques", "Jacson", "Jacy", "Jafar", "Jagger", "Jake", "Jal", "Jaleel",
				"Jamal", "James", "Jamison", "Jared", "Jarek", "Jarman", "Jaron", "Jarrod", "Jarvis", "Jason", "Jasper", "Javan", "Javier",
				"Jay", "Jebidiah", "Jed", "Jedidiah", "Jedrek", "Jeff", "Jeffrey", "Jelani", "Jeremiah", "Jeremy", "Jerolin", "Jerome", "Jeromy",
				"Jerzy", "Jesse", "Jessee", "Jethro", "Jibril", "Jin", "Jiro", "Jivin", "Joel", "Johann", "John", "Jolon", "Jonah", "Jonathan",
				"Jonathon", "Jordan", "Jordon", "Jorgen", "Jorvin", "Joseph", "Joshua", "Judd", "Jude", "Julian", "Julius", "Juma", "Jung",
				"Justin", "Kadin", "Kai", "Kaikara", "Kaladin", "Kalb", "Kale", "Kalil", "Kalkin", "Kalman", "Kamal", "Kane", "Kaniel", "Kardal",
				"Karl", "Karsten", "Kasch", "Kasen", "Kaspar", "Kateb", "Kayin", "Keane", "Kearney", "Kedar", "Keefe", "Keelan", "Keenan",
				"Kegan", "Keir", "Keir", "Keith", "Kelby", "Keleman", "Kell", "Kellen", "Kelvin", "Ken", "Kenan", "Kendall", "Kendrick", "Kenelm",
				"Kenley", "Kennard", "Kennedy", "Kenneth", "Kent", "Kenton", "Kenyon", "Keona", "Ker", "Kerby", "Kern", "Kerry", "Kers", "Kersen",
				"Kerwin", "Kester", "Kevin", "Khalil", "Khoury", "Kiefer", "Kieran", "Kiernan", "Killian", "Kin", "Kinnel", "Kinsey", "Kintan",
				"Kip", "Kirby", "Kirk", "Kiyoshi", "Kliftin", "Klog", "Komor", "Kontar", "Krischan", "Krister", "Kurt", "Kyle", "Kyler", "Laethan",
				"Laird", "Lamar", "Lamont", "Lance", "Lander", "Landon", "Lane", "Lang", "Larry", "Lars", "Lawler", "Lawrence", "Lazarus",
				"Lear", "Lee", "Leif", "Leighton", "Leland", "Len", "Lennon", "Lennor", "Lennox", "Lensar", "Leo", "Leon", "Leonard", "Leron",
				"Leroy", "Lester", "Lev", "Levi", "Lewis", "Lewis", "Li", "Liam", "Like", "Lincoln", "Lindsey", "Lionel", "Llewellyn", "Lloyd",
				"Logan", "Loren", "Lorenzo", "Lorne", "Louis", "Lowell", "Lucas", "Lucian", "Luis", "Luke", "Lukyan", "Lunt", "Luther", "Lyle",
				"Lyndon", "Lysander", "Mac", "Macer", "Mack", "Mackenzie", "Magnus", "Malcolm", "Malik", "Manco", "Mandek", "Mander", "Manfred",
				"Manning", "Mansur", "Manuel", "Marc", "Marcos", "Marcus", "Marden", "Marek", "Mario", "Mark", "Markham", "Markos", "Marlin",
				"Marlon", "Marlon", "Marshal", "Marshall", "Marsten", "Martin", "Martingo", "Marvin", "Mason", "Matai", "Mateo", "Mather",
				"Matthew", "Matthias", "Maurice", "Max", "Maxwell", "Maynard", "Mayon", "Mead", "Meka", "Mercer", "Merill", "Merle", "Merrick",
				"Merrik", "Meyer", "Micael", "Michael", "Migon", "Miguel", "Mike", "Mikkel", "Mikos", "Miles", "Miles", "Milo", "Milton",
				"Miner", "Mitchell", "Monroe", "Monte", "Morgan", "Morley", "Morris", "Mortimer", "Morton", "Morven", "Morz", "Motega", "Mukasa",
				"Murdoch", "Murdock", "Murphy", "Myles", "Myron", "Naeem", "Nalren", "Nantan", "Nathan", "Nathaniel", "Neal", "Neale", "Neil",
				"Nelek", "Nelson", "Neron", "Nestor", "Nevan", "Neville", "Nevin", "Nevin", "Nicanor", "Nicholas", "Nigel", "Nikolos", "Nils",
				"Noah", "Nodin", "Noe", "Nolan", "Norbert", "Norman", "Norris", "Norton", "Nuri", "Nyle", "Oakes", "Oakley", "Ochen", "Octavius",
				"Odell", "Odin", "Odion", "Odon", "Ogden", "Olaf", "Olin", "Oliver", "Omar", "Ordano", "Oren", "Orion", "Orman", "Ormand",
				"Orrin", "Orson", "Orville", "Oscar", "Osgood", "Osmond", "Otis", "Otto", "Owen", "Paco", "Palmer", "Paolo", "Paris", "Parker",
				"Parnell", "Pascal", "Patamon", "Patrick", "Patterson", "Patton", "Paul", "Paulin", "Pavel", "Paxton", "Payton", "Pearce",
				"Peder", "Pembroke", "Penn", "Percival", "Perry", "Peter", "Peyton", "Phearcy", "Philip", "Phillip", "Phillippe", "Phoenix",
				"Pierce", "Pierre", "Pierson", "Pilan", "Platon", "Porter", "Prentice", "Prescot", "Prescott", "Preston", "Quentin", "Quenton",
				"Quillan", "Quincy", "Quinlan", "Quinn", "Rad", "Radcliffe", "Radman", "Rafael", "Rafferty", "Ragnar", "Raidon", "Raleigh",
				"Ralph", "Ramiro", "Ramon", "Ramsay", "Ramsey", "Ranard", "Rance", "Randall", "Randolph", "Ranen", "Ranger", "Rankin", "Raoul",
				"Raphael", "Raul", "Ravi", "Ravi", "Ravid", "Ray", "Raymond", "Raynor", "Reade", "Redford", "Redmond", "Reed", "Reese", "Reeve",
				"Regan", "Reginald", "Regis", "Remington", "Renaldo", "Rendor", "Renfry", "Renny", "Reuben", "Rex", "Reyhan", "Rhett", "Rhett",
				"Rhys", "Ricardo", "Richard", "Richter", "Rico", "Rider", "Ridgley", "Rigby", "Riley", "Rimon", "Ringo", "Ringo", "Riodan",
				"Riordan", "Roarke", "Robert", "Roberto", "Robi", "Rockwell", "Rod", "Roderick", "Rodman", "Rodney", "Rodrigo", "Roger", "Roi",
				"Roland", "Roldan", "Rolf", "Ronald", "Ronan", "Rooney", "Rory", "Roscoe", "Ross", "Roth", "Rowan", "Rowland", "Roy", "Royce",
				"Ruben", "Rudd", "Rudi", "Rudyard", "Rufus", "Runako", "Ruskin", "Russ", "Russell", "Rusty", "Rutherford", "Rutledge", "Ryan",
				"Ryder", "Rylan", "Sahale", "Sahen", "Salim", "Saloman", "Sam", "Samien", "Sammon", "Samson", "Samuel", "Sanders", "Sandon",
				"Sandor", "Sanford", "Sargent", "Sarngin", "Sarojin", "Saul", "Saunders", "Sawyer", "Saxon", "Schuyler", "Scott", "Sean",
				"Sebastian", "Sebastien", "Seif", "Selby", "Senon", "Sergio", "Seth", "Seung", "Severin", "Sevilin", "Seward", "Seymour",
				"Shane", "Shawn", "Shea", "Sheffield", "Sheldon", "Shen", "Sheridan", "Sherman", "Sherwin", "Sherwood", "Shing", "Shunnar",
				"Sidney", "Siegfried", "Silas", "Simon", "Sivan", "Skip", "Skyler", "Slade", "Slevin", "Smith", "Solomon", "Sorgan", "Soterios",
				"Spalding", "Spencer", "Spenser", "Standford", "Stanley", "Stanton", "Stasio", "Stefan", "Stephan", "Stephen", "Sterling",
				"Stevan", "Steve", "Steven", "Stewart", "Stoke", "Stoyan", "Strom", "Stuart", "Subrey", "Sulaiman", "Sullican", "Sumner",
				"Sutherland", "Sutton", "Sven", "Sylvester", "Tab", "Tabari", "Tad", "Tadi", "Tai", "Tajo", "Talbart", "Talbot", "Talman",
				"Talos", "Tanek", "Tanner", "Tano", "Taro", "Tate", "Taurin", "Taylor", "Tem", "Terence", "Terrence", "Terrill", "Terry",
				"Thaddeus", "Thai", "Thaman", "Thane", "Thanos", "Theobald", "Theodore", "Theron", "Thierry", "Thomas", "Thorpe", "Thurston",
				"Thurston", "Tibalt", "Tiernan", "Timothy", "Titus", "Tobias", "Toby", "Tod", "Todd", "Tomas", "Tong", "Tor", "Torin", "Torrance",
				"Townsend", "Travers", "Travis", "Tremain", "Tremaine", "Trent", "Trevor", "Trey", "Tristan", "Troy", "Tryon", "Tucker", "Tully",
				"Tyee", "Tyler", "Tymon", "Tyrone", "Upton", "Uriah", "Urian", "Van", "Vance", "Vaughn", "Vern", "Vernon", "Victor", "Vincent",
				"Vinson", "Virgil", "Vito", "Vlad", "Vladimir", "Vokes", "Volf", "Wade", "Wagner", "Walden", "Waldo", "Walker", "Wallace",
				"Wally", "Walter", "Ward", "Warner", "Warren", "Watson", "Waylan", "Wayland", "Waylon", "Wayne", "Webb", "Webster", "Wendell",
				"Wesley", "Weston", "Weylin", "Whitaker", "Wilfen", "Will", "Willard", "Willem", "William", "Wilson", "Winston", "Winthrop",
				"Wlby", "Woody", "Wyatt", "Xavier", "Xenos", "Xerxes", "Ximen", "Yakecan", "Yale", "Yancey", "Yardley", "Yarin", "Yerik",
				"Yero", "Yervant", "York", "Yusuf", "Yves", "Zachariah", "Zachary", "Zackery", "Zaid", "Zaide", "Zane", "Zaniel", "Zann",
				"Zared", "Zarek", "Zeke", "Zenon", "Zion", "Ziven", "Zorn",
			}),

			#endregion

			#endregion
		};

		private static readonly Dictionary<string, NameList> m_Table = new(m_Lists.Length, StringComparer.InvariantCultureIgnoreCase);

		static NameList()
		{
			foreach (var list in m_Lists)
			{
				m_Table[list.Type] = list;
			}
		}

		public static NameList GetNameList(string type)
		{
			m_Table.TryGetValue(type, out var n);

			return n;
		}

		public static string RandomName(string type)
		{
			var list = GetNameList(type);

			if (list != null)
			{
				return list.GetRandomName();
			}

			return String.Empty;
		}

		private readonly string m_Type;
		private readonly string[] m_List;

		public string Type => m_Type;
		public string[] List => m_List;

		public bool ContainsName(string name)
		{
			for (var i = 0; i < m_List.Length; i++)
			{
				if (String.Equals(m_List[i], name, StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}

			return false;
		}

		public NameList(string type, string[] names)
		{
			m_Type = type;

			var unique = new HashSet<string>(names.Length, StringComparer.InvariantCultureIgnoreCase);

			unique.UnionWith(names);

			m_List = unique.ToArray();

			unique.Clear();
			unique.TrimExcess();

			for (var i = 0; i < m_List.Length; ++i)
			{
				m_List[i] = Utility.Intern(m_List[i].Trim());
			}
		}

		public string GetRandomName()
		{
			if (m_List.Length > 0)
			{
				return Utility.RandomList(m_List);
			}

			return String.Empty;
		}

		public IEnumerator<string> GetEnumerator()
		{
			for (var i = 0; i < m_List.Length; i++)
			{
				yield return m_List[i];
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}

namespace Server.Misc
{
	public class NameVerification
	{
		public static readonly char[] SpaceDashPeriodQuote = new char[]
			{
				' ', '-', '.', '\''
			};

		public static readonly char[] Empty = new char[0];

		public static void Initialize()
		{
			CommandSystem.Register("ValidateName", AccessLevel.Administrator, new CommandEventHandler(ValidateName_OnCommand));
		}

		[Usage("ValidateName")]
		[Description("Checks the result of NameValidation on the specified name.")]
		public static void ValidateName_OnCommand(CommandEventArgs e)
		{
			if (Validate(e.ArgString, 2, 16, true, false, true, 1, SpaceDashPeriodQuote))
			{
				e.Mobile.SendMessage(0x59, "That name is considered valid.");
			}
			else
			{
				e.Mobile.SendMessage(0x22, "That name is considered invalid.");
			}
		}

		public static bool Validate(string name, int minLength, int maxLength, bool allowLetters, bool allowDigits, bool noExceptionsAtStart, int maxExceptions, char[] exceptions)
		{
			return Validate(name, minLength, maxLength, allowLetters, allowDigits, noExceptionsAtStart, maxExceptions, exceptions, m_Disallowed, m_StartDisallowed);
		}

		public static bool Validate(string name, int minLength, int maxLength, bool allowLetters, bool allowDigits, bool noExceptionsAtStart, int maxExceptions, char[] exceptions, string[] disallowed, string[] startDisallowed)
		{
			if (name == null || name.Length < minLength || name.Length > maxLength)
			{
				return false;
			}

			var exceptCount = 0;

			name = name.ToLower();

			if (!allowLetters || !allowDigits || (exceptions.Length > 0 && (noExceptionsAtStart || maxExceptions < Int32.MaxValue)))
			{
				for (var i = 0; i < name.Length; ++i)
				{
					var c = name[i];

					if (c >= 'a' && c <= 'z')
					{
						if (!allowLetters)
						{
							return false;
						}

						exceptCount = 0;
					}
					else if (c >= '0' && c <= '9')
					{
						if (!allowDigits)
						{
							return false;
						}

						exceptCount = 0;
					}
					else
					{
						var except = false;

						for (var j = 0; !except && j < exceptions.Length; ++j)
						{
							if (c == exceptions[j])
							{
								except = true;
							}
						}

						if (!except || (i == 0 && noExceptionsAtStart))
						{
							return false;
						}

						if (exceptCount++ == maxExceptions)
						{
							return false;
						}
					}
				}
			}

			for (var i = 0; i < disallowed.Length; ++i)
			{
				var indexOf = name.IndexOf(disallowed[i]);

				if (indexOf == -1)
				{
					continue;
				}

				var badPrefix = (indexOf == 0);

				for (var j = 0; !badPrefix && j < exceptions.Length; ++j)
				{
					badPrefix = (name[indexOf - 1] == exceptions[j]);
				}

				if (!badPrefix)
				{
					continue;
				}

				var badSuffix = ((indexOf + disallowed[i].Length) >= name.Length);

				for (var j = 0; !badSuffix && j < exceptions.Length; ++j)
				{
					badSuffix = (name[indexOf + disallowed[i].Length] == exceptions[j]);
				}

				if (badSuffix)
				{
					return false;
				}
			}

			for (var i = 0; i < startDisallowed.Length; ++i)
			{
				if (name.StartsWith(startDisallowed[i]))
				{
					return false;
				}
			}

			return true;
		}

		public static string[] StartDisallowed => m_StartDisallowed;
		public static string[] Disallowed => m_Disallowed;

		private static readonly string[] m_StartDisallowed = new string[]
			{
				"seer",
				"counselor",
				"gm",
				"admin",
				"lady",
				"lord"
			};

		private static readonly string[] m_Disallowed = new string[]
			{
				"jigaboo",
				"chigaboo",
				"wop",
				"kyke",
				"kike",
				"tit",
				"spic",
				"prick",
				"piss",
				"lezbo",
				"lesbo",
				"felatio",
				"dyke",
				"dildo",
				"chinc",
				"chink",
				"cunnilingus",
				"cum",
				"cocksucker",
				"cock",
				"clitoris",
				"clit",
				"ass",
				"hitler",
				"penis",
				"nigga",
				"nigger",
				"klit",
				"kunt",
				"jiz",
				"jism",
				"jerkoff",
				"jackoff",
				"goddamn",
				"fag",
				"blowjob",
				"bitch",
				"asshole",
				"dick",
				"pussy",
				"snatch",
				"cunt",
				"twat",
				"shit",
				"fuck",
				"tailor",
				"smith",
				"scholar",
				"rogue",
				"novice",
				"neophyte",
				"merchant",
				"medium",
				"master",
				"mage",
				"lb",
				"journeyman",
				"grandmaster",
				"fisherman",
				"expert",
				"chef",
				"carpenter",
				"british",
				"blackthorne",
				"blackthorn",
				"beggar",
				"archer",
				"apprentice",
				"adept",
				"gamemaster",
				"frozen",
				"squelched",
				"invulnerable",
				"osi",
				"origin"
			};
	}

	public class RenameRequests
	{
		public static void Initialize()
		{
			EventSink.RenameRequest += new RenameRequestEventHandler(EventSink_RenameRequest);
		}

		private static void EventSink_RenameRequest(RenameRequestEventArgs e)
		{
			var from = e.From;
			var targ = e.Target;
			var name = e.Name;

			if (from.CanSee(targ) && from.InRange(targ, 12) && targ.CanBeRenamedBy(from))
			{
				name = name.Trim();

				if (NameVerification.Validate(name, 1, 16, true, false, true, 0, NameVerification.Empty, NameVerification.StartDisallowed, (Core.ML ? NameVerification.Disallowed : new string[] { })))
				{

					if (Core.ML)
					{
						var disallowed = ProfanityProtection.Disallowed;

						for (var i = 0; i < disallowed.Length; i++)
						{
							if (name.IndexOf(disallowed[i]) != -1)
							{
								from.SendLocalizedMessage(1072622); // That name isn't very polite.
								return;
							}
						}

						from.SendLocalizedMessage(1072623, String.Format("{0}\t{1}", targ.Name, name)); // Pet ~1_OLDPETNAME~ renamed to ~2_NEWPETNAME~.
					}

					targ.Name = name;
				}
				else
				{
					from.SendMessage("That name is unacceptable.");
				}
			}
		}
	}
}