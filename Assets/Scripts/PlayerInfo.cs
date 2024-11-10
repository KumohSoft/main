using UnityEngine;
using Firebase.Firestore;
[FirestoreData]//Firestore에서 인식하기 위해 꼭 적어줘야함.
public struct PlayerInfo
{
    [FirestoreProperty]
    public string NickName { get; set; }
}
