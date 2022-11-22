using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsLocalNetwork.Constants
{
    internal class Constant
    {
        public const string clientID = "779390058368-dbrufvjtoinvkbbmt2c3a7n6gii84gaj.apps.googleusercontent.com";
        public const string clientSecret = "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";
        public const string authorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
        public const string tokenEndpoint = "https://www.googleapis.com/oauth2/v4/token";
        public const string userInfoEndpoint = "https://www.googleapis.com/oauth2/v3/userinfo";

        public const string FOLDER_IN_GOOGLE_DRIVE = "1CVgiDasfaWCPGCdkUpStx3YNmylCxmm4";
        public static string BASE_FOLDER = Directory.GetCurrentDirectory();
    }
}
