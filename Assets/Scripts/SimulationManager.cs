using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimulationManager : MonoBehaviour
{
   [SerializeField] private List<ParkingLot> parkingLots;
   [SerializeField] private List<GameObject> carPrefabs;
   [SerializeField] private AutoParkAgent agent;

   private List<GameObject> parkedCars;
   
   private  float spawnXMin = -2f;
   private  float spawnXMax = 2f;
   private  float spawnZMin = -5f;
   private  float spawnZMax = -3f;
   private bool _initComplete = false;

   public bool InitComplete => _initComplete;

   private void Awake()
   {
      parkedCars = new List<GameObject>();
   }

   public void InitializeSimulation()
   {
      _initComplete = false;
      StartCoroutine(OccupyParkingSlotsWithRandomCars());
      RepositionAgentRandom();
   }

   public void RepositionAgentRandom()
   {
      if (agent != null)
      {
         agent.GetComponent<Rigidbody>().velocity = Vector3.zero;
         agent.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
         agent.GetComponent<CarController>().CurrentSteeringAngle = 0f;
         agent.GetComponent<CarController>().CurrentAcceleration = 0f;
         agent.GetComponent<CarController>().CurrentBrakeTorque = 0f;
         agent.transform.rotation = Quaternion.Euler(0,180,0);
         agent.transform.position = transform.parent.position + new Vector3(Random.Range(spawnXMin,spawnXMax),-0.61f,Random.Range(spawnZMin,spawnZMax));
      }
   }

   public void ResetSimulation()
   {
      foreach (GameObject parkedCar in parkedCars)
      {
         Destroy(parkedCar);
      }

      foreach (ParkingLot parkingLot in parkingLots)
      {
         parkingLot.IsOccupied = false;
      }
      parkedCars.Clear();
   }

   public IEnumerator OccupyParkingSlotsWithRandomCars()
   {
      foreach (ParkingLot parkingLot in parkingLots)
      {
         parkingLot.IsOccupied = false;
      }
      yield return new WaitForSeconds(1);

      int total = Random.Range(10, 19);
      for (int i = 0; i < total; i++)
      {
         ParkingLot lot = parkingLots.Where(r => r.IsOccupied == false).OrderBy(r => Guid.NewGuid()).FirstOrDefault();
         if (lot != null)
         {
            GameObject carInstance = Instantiate(carPrefabs[Random.Range(0, 3)]);
            carInstance.transform.position = new Vector3(lot.transform.position.x, 0f, lot.transform.position.z);
            parkedCars.Add(carInstance);
            lot.IsOccupied = true;
            if(parkedCars.Count >= total)
               break;
         }
      }

      _initComplete = true;
      
      if(Random.Range(0f,1f) > 0.5f)
         PositionAtSafePlace(GetRandomEmptyParkingSlot().gameObject);
   }

   public ParkingLot GetRandomEmptyParkingSlot()
   {
     return parkingLots.Where(r => r.IsOccupied == false).OrderBy(r => Guid.NewGuid())
         .FirstOrDefault();
   }
   public void PositionAtSafePlace(GameObject nearestLotGameObject)
   {
      float[] ang = new float[] {-90f, 90f, 180f, -180f,0f};
      
      if (agent != null)
      {
         agent.GetComponent<Rigidbody>().velocity = Vector3.zero;
            agent.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            agent.GetComponent<CarController>().CurrentSteeringAngle = 0f;
            agent.GetComponent<CarController>().CurrentAcceleration = 0f;
            agent.GetComponent<CarController>().CurrentBrakeTorque = 0f;
            Vector3 newPosition = nearestLotGameObject.transform.position +
                                  nearestLotGameObject.transform.right * Random.Range(-3f, -7f) +
                                  nearestLotGameObject.transform.forward * Random.Range(-1f, 1f);
            agent.transform.position = newPosition;
            agent.transform.Rotate(agent.transform.up,ang[Random.Range(0,4)]);
      }
   }
}
