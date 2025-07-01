using System.Text.RegularExpressions;
using XIVUniPF_Core;

namespace XIVUniPF.Classes.Filters
{
    partial class SearchBoxFilter
    {
        public bool Predict(PartyInfo info, string keywords)
        {
            Regex dutyReg = DutyReg();
            Regex idReg = IdReg();
            Regex dcReg = DcReg();
            Regex serverReg = WorldReg();

            var kws = keywords.Trim().Split(' ');
            foreach (var kw in kws)
            {
                if (kw == string.Empty)
                    return true;

                // 匹配：副本:xxx
                var dutyMatch = dutyReg.Match(kw);
                if (dutyMatch.Success && info.Duty.Contains(dutyMatch.Groups[1].Value))
                    return true;

                // 匹配：id:xxx
                var idMatch = idReg.Match(kw);
                if (idMatch.Success && info.Name.Contains(idMatch.Groups[1].Value))
                    return true;

                // 匹配：大区:xxx
                var dcMatch = dcReg.Match(kw);
                if (dcMatch.Success && info.Datacenter.Contains(dcMatch.Groups[1].Value))
                    return true;

                // 匹配：服务器:xxx
                var serverMatch = serverReg.Match(kw);
                if (serverMatch.Success && info.Home_world.Contains(serverMatch.Groups[1].Value))
                    return true;

                // 普通关键字匹配
                if (info.Description.Contains(kw))
                    return true;
            }

            return false;
        }

        [GeneratedRegex(@"副本[：:](.+)")]
        private static partial Regex DutyReg();

        [GeneratedRegex(@"id[：:](.+)", RegexOptions.IgnoreCase, "zh-CN")]
        private static partial Regex IdReg();

        [GeneratedRegex(@"大区[：:](.+)")]
        private static partial Regex DcReg();

        [GeneratedRegex(@"服务器[：:](.+)")]
        private static partial Regex WorldReg();
    }
}
