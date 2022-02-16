using UnityEngine;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;
using Photon.Pun;
using ExitGames.Client.Photon;
using System;

public class UIPlayerSelection : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text _usernameText;
    [SerializeField] private Player _owner;
    [SerializeField] private GameObject _kickButton;

    private const string CHARACTER_SELECTION_NUMBER = "CSN";
    private const string KICKED_PLAYER = "KICKED";

    public static Action<Player> OnKickPlayer = delegate { };

    public Player Owner
    {
        get { return _owner; }
        private set { _owner = value; }
    }

    public void Initialize(Player player)
    {
        Debug.Log($"Player Selection Init {player.NickName}");
        Owner = player;
        SetupPlayerSelection();
    }

    #region Private Methods
    private void SetupPlayerSelection()
    {
        _usernameText.SetText(_owner.NickName);
        _kickButton.SetActive(false);

        if (PhotonNetwork.IsMasterClient)
        {
            ShowMasterClientUI();
        }
    }

    private void ShowMasterClientUI()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (PhotonNetwork.LocalPlayer.Equals(Owner))
        {
            _kickButton.SetActive(false);
        }
        else
        {
            _kickButton.SetActive(true);
        }
    }

    private int GetCharacterSelection()
    {
        int selection = 0;
        object playerSelectionObj;
        if (Owner.CustomProperties.TryGetValue(CHARACTER_SELECTION_NUMBER, out playerSelectionObj))
        {
            selection = (int)playerSelectionObj;
        }
        return selection;
    }

    private void UpdateCharacterSelection(int selection)
    {
        Debug.Log($"Updating Photon Custom Property {CHARACTER_SELECTION_NUMBER} for {PhotonNetwork.LocalPlayer.NickName} to {selection}");

        Hashtable playerSelectionProperty = new Hashtable()
            {
                {CHARACTER_SELECTION_NUMBER, selection}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProperty);
    }
    #endregion

    #region Private Methods

    public void KickPlayer()
    {
        Debug.Log($"Updating Photon Custom Property {CHARACTER_SELECTION_NUMBER} for {Owner} to {true}");

        Hashtable kickedProperty = new Hashtable()
            {
                {KICKED_PLAYER, true}
            };
        Owner.SetCustomProperties(kickedProperty);
    }
    #endregion

    #region Photon Callback Methods
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (Owner.Equals(newMasterClient))
        {
            ShowMasterClientUI();
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!Owner.Equals(targetPlayer)) return;

        object kickedPlayerObject;
        if (changedProps.TryGetValue(KICKED_PLAYER, out kickedPlayerObject))
        {
            bool kickedPlayer = (bool)kickedPlayerObject;
            if (kickedPlayer)
            {
                Hashtable kickedProperty = new Hashtable()
                    {
                        {KICKED_PLAYER, false}
                    };
                Owner.SetCustomProperties(kickedProperty);

                OnKickPlayer?.Invoke(Owner);
            }
        }
    }
    #endregion
}