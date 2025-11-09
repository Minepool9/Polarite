using Steamworks;

using UnityEngine;

namespace Polarite.Multiplayer
{
    public class NetworkEnemySync : MonoBehaviour
    {
        public string id;
        public bool here;
        public ulong owner;

        void Awake()
        {
            id = SceneObjectCache.GetScenePath(gameObject);
        }

        void OnEnable()
        {
            // have to use steam id here
            if(GetComponent<NetworkEnemy>() == null && NetworkManager.InLobby && GetComponent<NetworkPlayer>() == null)
            {
                NetworkEnemy.Create(id, GetComponent<EnemyIdentifier>(), (GetComponent<EnemyIdentifier>().isBoss) ? SteamClient.SteamId : NetworkManager.Instance.CurrentLobby.Owner.Id);
            }
            if (NetworkManager.InLobby && !here && GetComponent<NetworkPlayer>() == null)
            {
                here = true;
                owner = (GetComponent<EnemyIdentifier>().isBoss) ? SteamClient.SteamId : NetworkManager.Instance.CurrentLobby.Owner.Id;
                PacketWriter w = new PacketWriter();
                w.WriteString(id);
                NetworkManager.Instance.BroadcastPacket(PacketType.EnemySpawn, w.GetBytes());
            }
        }
    }
}