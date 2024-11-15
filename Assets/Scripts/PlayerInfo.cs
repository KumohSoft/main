using UnityEngine;
using Firebase.Firestore;
[FirestoreData]//Firestore에서 인식하기 위해 꼭 적어줘야함.
public struct PlayerInfo
{
    [FirestoreProperty]
    public string NickName { get; set; }
    [FirestoreProperty]
    public int Gold { get; set; }
    [FirestoreProperty]
    public int WinCount { get; set; }
    [FirestoreProperty]
    public int LoseCount { get; set; }
    [FirestoreProperty]
    public int Level { get; set; }
    [FirestoreProperty]
    public int []Character { get; set; }
    [FirestoreProperty]
    public int []Item { get; set; }

}
