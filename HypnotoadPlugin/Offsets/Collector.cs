/*
 * Copyright(c) 2024 GiR-Zippo, Meowchestra
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using Dalamud.Game.ClientState.Party;
using System;
using System.Collections.Generic;
using Dalamud.Plugin.Services;

namespace HypnotoadPlugin.Offsets
{
    public struct playerInfo
    {
        // Struct for holding the playerInfo relevent to our purposes. These values
        // are fetched using the various methods throughout the PartyHandler class
        public playerInfo(string name, string world, string region)
        {
            playerName = name;
            playerWorld = world;
            playerRegion = region;
        }
        public string playerName { get; }
        public string playerWorld { get; }
        public string playerRegion { get; }
        public override string ToString() => $"{playerName} [{playerWorld}, {playerRegion}]";
    }

    public class Collector
    {
        #region Const/Dest
        private static readonly Lazy<Collector> LazyInstance = new(() => new Collector());

        private Collector()
        {
        }

        public static Collector Instance => LazyInstance.Value;

       
        public void Initialize(IDataManager data, IClientState clientState, IPartyList partyList)
        {
            Data = data;
            ClientState = clientState;
            PartyList = partyList;
            ClientState.Login += ClientState_Login;
            ClientState.Logout += ClientState_Logout;
        }

        ~Collector() => Dispose();
        public void Dispose()
        {
            ClientState.Login -= ClientState_Login;
            ClientState.Logout -= ClientState_Logout;
            GC.SuppressFinalize(this);
        }
        #endregion

        internal IDataManager Data;
        internal IClientState ClientState;
        internal IPartyList PartyList;

        /// <summary>
        /// Only called when the plugin is started
        /// </summary>
        public void UpdateClientStats()
        {
            ClientState_Login();
        }

        /// <summary>
        /// Triggered by ClientState_Login
        /// Send the Name and WorldId to the LA
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientState_Login()
        {
            if (Api.GetLocalPlayer() != null)
            {
                var Name = Api.GetLocalPlayer().Name.TextValue;
                var HomeWorld = Api.GetLocalPlayer().HomeWorld.ValueNullable?.RowId;
                if (Pipe.Client != null && Pipe.Client.IsConnected)
                {
                    Pipe.Client.WriteAsync(new IPCMessage
                    {
                        msgType = MessageType.NameAndHomeWorld,
                        message = Environment.ProcessId + ":" + Name + ":" + HomeWorld.ToString()
                    });
                }
            }
        }

        private void ClientState_Logout(int type, int code)
        {
        }


        private List<playerInfo> _getInfoFromNormalParty()
        {
            // Generates a list of playerInfo objects from the game's memory
            // assuming the party is a normal party (light/full/etc.)
            string tempName;
            string tempWorld;
            string tempRegion;
            List<playerInfo> output = new List<playerInfo>();
            int pCount = PartyList.Length;

            for (int i = 0; i < pCount; i++)
            {
                IntPtr memberPtr = PartyList.GetPartyMemberAddress(i);
                IPartyMember member = PartyList.CreatePartyMemberReference(memberPtr);
                tempName = member.Name.ToString();
                tempWorld = ""; // worldNameFromByte((byte)member.World.Id);
                tempRegion = ""; // regionFromWorld(tempWorld);
                output.Add(new playerInfo(tempName, tempWorld, tempRegion));
            }
            return output;
        }
    }
}
