using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using WokBot.Interfaces;

namespace WokBot.Commands
{
    public class SecurityModule : ModuleBase<SocketCommandContext>
    {
        [Command("security")]

        public async Task SayAsync(string link)
        {
            var goodEmoji = new Emoji("👍");
            var badEmoji = new Emoji("👎");
            string url = "https://www.virustotal.com/vtapi/v2/url/report?apikey=" + Program.resourcesInterface.virus_total + "&resource=" + link;

            string html = string.Empty;

            //Virus_total result = Program.utility.ApiCall<Virus_total>(url);
            virusInterface result = await Program.utility.ApiCall<virusInterface>(url);

            // Checks against Kaspersky, Sophos & Google Safe Browsing
            // If it fails any, it will let the user know
            if ((result.scans.Kaspersky != null && result.scans.Kaspersky.detected)
                || (result.scans.Sophos != null && result.scans.Sophos.detected)
                || (result.scans.GoogleSafebrowsing != null && result.scans.GoogleSafebrowsing.detected))
            {
                await Context.Message.AddReactionAsync(badEmoji);
            }
            else
            {
                await Context.Message.AddReactionAsync(goodEmoji);
            }
        }
    }
}
