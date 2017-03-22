using System;
using System.Collections.Generic;

namespace CCNet.Build.Reconfigure
{
	public class BuildOwners
	{
		private readonly Dictionary<string, string> m_emails;
		private readonly Dictionary<string, string> m_aliases;

		public BuildOwners()
		{
			m_emails = new Dictionary<string, string>();
			m_aliases = new Dictionary<string, string>();

			AddUser("8a99855552936a300152936cc3cf736d", "andrei.barbolin@cbsinteractive.com", "abarbolin");
			AddUser("8a99855552936a300152936cbec64c67", "andrei.golyakov@cbsinteractive.com", "agolyakov");
			AddUser("8a99855552936a300152936cc3ee745f", "alexander.ageev@cbsinteractive.com", "aageev");
			AddUser("8a99855552936a300152936cb7cf1681", "alexander.kovalenko@cbsinteractive.com", "akovalenko");
			AddUser("8a99855552936a300152936cb0a65fbd", "anton.lisitsyn@cbsinteractive.com", "alisitsyn");
			AddUser("8a99855552936a300152936ca5340e71", "artem.ryzhkov@cbsinteractive.com", "aryzhkov");
			AddUser("8a99855552936a300152936ca3210228", "dmitri.baranov@cbsinteractive.com", "dbaranov");
			AddUser("8a99855553d630f90157d22184df027e", "dmitri.egurnov@cbsinteractive.com", "degurnov");
			AddUser("8a99855552936a300152936cbc1c37ac", "dmitri.semikolenov@cbsinteractive.com", "dsemikolenov");
			AddUser("8a99855552936a300152936cbd654199", "ekaterina.marchenkova@cbsinteractive.com ", "emarchenkova");
			AddUser("8a99855552936a300152936cbbb53498", "eugeni.elokhov@cbsinteractive.com", "eelokhov");
			AddUser("8a99855552936a300152936cb4727cd2", "eugeni.popodyanets@cbsinteractive.com", "epopodyanets");
			AddUser("8a99855552936a300152936cbb8f334d", "igor.rybin@cbsinteractive.com", "irybin");
			AddUser("8a99855552936a300152936cb588051e", "irina.tirskaya@cbsinteractive.com", "itirskaya");
			AddUser("8a99855552936a300152936cc08259d7", "kirill.luzin@cbsinteractive.com", "kluzin");
			AddUser("8a99855552936a300152936cb6400a8a", "maria.melnikova@cbsinteractive.com", "mmelnikova");
			AddUser("8a99855552936a300152936caaf434a9", "natalia.savelieva@cbsinteractive.com", "nsavelieva");
			AddUser("8a99855552936a300152936cc2836991", "nikita.serduk@cbsinteractive.com", "nserduk");
			AddUser("8a99855552936a300152936cadf74b66", "oleg.shuruev@cbsinteractive.com", "oshuruev", "olshuruev");
			AddUser("8a99855552936a300152936cbe1246f1", "pavel.belousov@cbsinteractive.com", "pbelousov");
			AddUser("8a99855552936a300152936cb51901bc", "pavel.kashirin@cbsinteractive.com", "pkashirin");
			AddUser("8a99855552936a300152936cb15d6557", "pavel.svintsov@cbsinteractive.com", "psvintsov");
			AddUser("8a998555590d8615015a46f8815a011c", "pavel.urvachev@cbsinteractive.com", "purvachev");
			AddUser("8a99855552936a300152936ca5f61316", "roman.pusenkov@cbsinteractive.com", "rpusenkov");
			AddUser("8a99855552936a300152936cb4a77e8a", "sergey.kolemasov@cbsinteractive.com", "skolemasov", "kolemasovs");
			AddUser("8a99855552936a300152936cabe33b9b", "sergei.konkin@cbsinteractive.com", "skonkin");
			AddUser("8a99855552936a300152936caec65198", "valeri.ilyin@cbsinteractive.com", "vilyin");
			AddUser("8a99855552936a300152936cb85b1ac8", "vera.perfilieva@cbsinteractive.com", "vperfilieva");
		}

		private void AddUser(
			string uid,
			string email,
			params string[] aliases)
		{
			m_emails.Add(uid, email);
			m_aliases.Add(uid, uid);

			foreach (var alias in aliases)
			{
				m_aliases.Add(alias, uid);
			}
		}

		public string GetUid(string user)
		{
			if (m_aliases.ContainsKey(user))
				return m_aliases[user];

			throw new InvalidOperationException($"Unknown user '{user}'.");
		}

		public string GetEmail(string uid)
		{
			if (m_emails.ContainsKey(uid))
				return m_emails[uid];

			throw new InvalidOperationException($"Unknown user UID = '{uid}'.");
		}
	}
}
