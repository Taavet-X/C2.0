using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Stores references to all of the possible rooms, in a list currently
/// sorted by room size.
/// Should probably later on be also be sorted by room type (shop, fight room, boss room, etc). 
/// But this should be fine for an alpha
/// </summary>
public class RoomGen : MonoBehaviour
{

    [SerializeField] bool onlyGenerateOneRoom = false;
    [SerializeField] GameObject oneRoomToGenerate;
    [SerializeField] List<GameObject> allRooms = new List<GameObject>();
    [SerializeField] Dictionary<RoomType, List<GameObject>> roomsByRoomType = new Dictionary<RoomType, List<GameObject>>();

    private void Awake(){
        InitializeRooms();
    }

    private void InitializeRooms(){
        roomsByRoomType.Clear();
        if (onlyGenerateOneRoom == true) {
            Debug.Log("Debug choice for only generating one type of room is on, disable this if not used on purpose");
        }
        for (int i = 0; i < allRooms.Count; i++){
            List<GameObject> listForRoomType;
            roomsByRoomType.TryGetValue(allRooms[i].GetComponent<Room>().RoomType, out listForRoomType);
            if (listForRoomType == null){
                listForRoomType = new List<GameObject>();
                roomsByRoomType[allRooms[i].GetComponent<Room>().RoomType] = listForRoomType;
            }
            listForRoomType.Add(allRooms[i]);
        }
    }

    public GameObject createRoomForCell(Cell cell){
        GameObject go = null;
        if (roomsByRoomType[cell.RoomType].Count <= 0) {
            InitializeRooms();
        }
        if (!onlyGenerateOneRoom){
            List<GameObject> roomsThatCanBeGeneratedHere = new List<GameObject>(roomsByRoomType[cell.RoomType]);
            foreach (Cell neighbor in cell.NeighborCells.Values){
                if (ProceduralGeneration.Singleton.AllCellsWithRooms.ContainsKey(neighbor)){
                    roomsThatCanBeGeneratedHere.Remove(ProceduralGeneration.Singleton.AllCellsWithRooms[neighbor].RoomPrefab);
                }
            }
            if (roomsThatCanBeGeneratedHere.Count <= 1){
                //Not enough rooms! let the generator make same rooms adjancent
                roomsThatCanBeGeneratedHere = new List<GameObject>(roomsByRoomType[cell.RoomType]);
            }
            int random = Random.Range(0, roomsThatCanBeGeneratedHere.Count);
            go = Instantiate(roomsThatCanBeGeneratedHere[random]);
            go.GetComponent<Room>().RoomPrefab = roomsThatCanBeGeneratedHere[random];
            roomsByRoomType[cell.RoomType].Remove(go.GetComponent<Room>().RoomPrefab);
        }
        else {
            go = Instantiate(oneRoomToGenerate);
        }
        go.GetComponent<Room>().Cell = cell;

        ProceduralGeneration.Singleton.AddCellToRoomInformation(cell, go.GetComponent<Room>());

        return go;
    }
}
