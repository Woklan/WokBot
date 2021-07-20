using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WokBot.Interfaces
{
    public class Vendor
    {
        public bool detected { get; set; }
        public string result { get; set; }
    }

    public class Scans
    {
        public Vendor CMCThreatIntelligence { get; set; }
        public Vendor VXVault { get; set; }
        public Vendor Armis { get; set; }
        public Vendor ComodoValkyrieVerdict { get; set; }
        public Vendor PhishLabs { get; set; }
        public Vendor K7AntiVirus { get; set; }
        public Vendor CINSArmy { get; set; }
        public Vendor Cyren { get; set; }
        public Vendor Quttera { get; set; }
        public Vendor BlockList { get; set; }
        public Vendor OpenPhish { get; set; }
        public Vendor FeodoTracker { get; set; }
        public Vendor WebSecurityGuard { get; set; }
        public Vendor Scantitan { get; set; }
        public Vendor AlienVault { get; set; }
        public Vendor Sophos { get; set; }
        public Vendor Phishtank { get; set; }
        public Vendor EonScope { get; set; }
        public Vendor Cyan { get; set; }
        public Vendor Spam404 { get; set; }
        public Vendor SecureBrain { get; set; }
        public Vendor HopliteIndustries { get; set; }
        public Vendor CRDF { get; set; }
        public Vendor Rising { get; set; }
        public Vendor StopForumSpam { get; set; }
        public Vendor Fortinet { get; set; }
        public Vendor AlphaMountainAi { get; set; }
        public Vendor VirusdieExternalSiteScan { get; set; }
        public Vendor ArtistsAgainst419 { get; set; }
        public Vendor GoogleSafebrowsing { get; set; }
        public Vendor SafeToOpen { get; set; }
        public Vendor ADMINUSLabs { get; set; }
        public Vendor CyberCrime { get; set; }
        public Vendor AutoShun { get; set; }
        public Vendor Trustwave { get; set; }
        public Vendor AICCMONITORAPP { get; set; }
        public Vendor CyRadar { get; set; }
        public Vendor DrWeb { get; set; }
        public Vendor Emsisoft { get; set; }
        public Vendor Webroot { get; set; }
        public Vendor Avira { get; set; }
        public Vendor CiscoTalosIPBlacklist { get; set; }
        public Vendor securolytics { get; set; }
        public Vendor AntiyAVL { get; set; }
        public Vendor AegisLabWebGuard { get; set; }
        public Vendor QuickHeal { get; set; }
        public Vendor CLEANMX { get; set; }
        public Vendor DNS8 { get; set; }
        public Vendor BenkowCc { get; set; }
        public Vendor EmergingThreats { get; set; }
        public Vendor YandexSafebrowsing { get; set; }
        public Vendor MalwareDomainList { get; set; }
        public Vendor Lumu { get; set; }
        public Vendor zvelo { get; set; }
        public Vendor Kaspersky { get; set; }
        public Vendor MalwareDomainBlocklist { get; set; }
        public Vendor DesenmascaraMe { get; set; }
        public Vendor URLhaus { get; set; }
        public Vendor PREBYTES { get; set; }
        public Vendor SucuriSiteCheck { get; set; }
        public Vendor Blueliv { get; set; }
        public Vendor Netcraft { get; set; }
        public Vendor ZeroCERT { get; set; }
        public Vendor PhishingDatabase { get; set; }
        public Vendor MalwarePatrol { get; set; }
        public Vendor MalBeacon { get; set; }
        public Vendor Sangfor { get; set; }
        public Vendor IPsum { get; set; }
        public Vendor Spamhaus { get; set; }
        public Vendor Malwared { get; set; }
        public Vendor BitDefender { get; set; }
        public Vendor GreenSnow { get; set; }
        public Vendor GData { get; set; }
        public Vendor StopBadware { get; set; }
        public Vendor SCUMWAREOrg { get; set; }
        public Vendor MalwaresComURLchecker { get; set; }
        public Vendor NotMining { get; set; }
        public Vendor ForcepointThreatSeeker { get; set; }
        public Vendor Certego { get; set; }
        public Vendor ESET { get; set; }
        public Vendor Threatsourcing { get; set; }
        public Vendor MalSilo { get; set; }
        public Vendor Nucleon { get; set; }
        public Vendor BADWAREINFO { get; set; }
        public Vendor ThreatHive { get; set; }
        public Vendor FraudScore { get; set; }
        public Vendor Tencent { get; set; }
        public Vendor BforeAiPreCrime { get; set; }
        public Vendor BaiduInternational { get; set; }
    }

    public class virusInterface
    {
        public string scan_id { get; set; }
        public string resource { get; set; }
        public string url { get; set; }
        public int response_code { get; set; }
        public string scan_date { get; set; }
        public string permalink { get; set; }
        public string verbose_msg { get; set; }
        public string filescan_id { get; set; }
        public int positives { get; set; }
        public int total { get; set; }
        public Scans scans { get; set; }
    }
}
