﻿using UnityEngine;

namespace WavingGrid
{
    public class GridManager : MonoBehaviour
    {
        private GameObject[,] cubesGrid;

        public int numRows = 10;
        public int numCols = 10;
        public Material cubeMat;
        public int SpringNeighbor = 10;
        public int SpringBase = 5;
        public float MaxDisplacement = 8;

        public bool isInteractive = true;

        // Use this for initialization
        void Start ()
        {
            CreateGrid();

            AddRowJoints();
            AddColJoints();

            DisableInteractive();
        }

        private void CreateGrid()
        {
            var gridPlane = GameObject.FindGameObjectWithTag("GridPlane");
            
            gridPlane.transform.localScale = new Vector3(numRows, 1, numCols);

            cubesGrid = new GameObject[numRows, numCols];

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    var cubePos = new Vector3(i * 1 + .5f, 0, j * 1 + 0.5f);
                    Vector3 position = cubePos;
                    GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);

                    cube1.GetComponent<Renderer>().material = cubeMat;
                    var cubeTransform = cube1.transform;

                    //move down by half cube height for scaling from one side
                    position += new Vector3(0, -0.5f, 0);

                    cubeTransform.position = position;

                    GameObject go = new GameObject("Scaler object");
                    go.transform.parent = transform;
                    cube1.transform.parent = go.transform;


                    var rb = cube1.AddComponent<Rigidbody>();
                    rb.useGravity = false;
                    rb.constraints = RigidbodyConstraints.FreezeRotation;

                    cube1.GetComponent<Collider>().enabled = false;
                    go.transform.localScale = new Vector3(1, MaxDisplacement, 1);

                    cube1.AddComponent<SpringJoint>().spring = SpringBase;

                    cube1.AddComponent<CubeMovement>();
                    GameObject cube = cube1;

                    cubesGrid[i, j] = cube;

                    //create quad collider for detecting pressure
                    var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);

                    PressureDetector pressureDetector = quad.AddComponent<PressureDetector>();
                    pressureDetector.Init(cube, MaxDisplacement);
                    cube.GetComponent<CubeMovement>().initialY = pressureDetector.initY;

                    quad.transform.position = cubePos;
                    quad.transform.Rotate(90,0,0);
                    quad.transform.parent = cube.transform.parent;
                    quad.GetComponent<Renderer>().enabled = false;
                }
            }
        }

        private void AddRowJoints()
        {
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols - 1; j++)
                {
                    LinkCubes(cubesGrid[i, j], cubesGrid[i, j + 1]);
                }
            }
        }

        private void AddColJoints()
        {
            for (int j = 0; j < numCols; j++)
            {
                for (int i = 0; i < numRows - 1; i++)
                {
                    LinkCubes(cubesGrid[i, j], cubesGrid[i + 1, j]);
                }
            }
        }

        private void LinkCubes(GameObject first, GameObject second)
        {
            var joint = first.AddComponent<SpringJoint>();
            joint.connectedBody = second.GetComponent<Rigidbody>();
            joint.spring = SpringNeighbor;
        }


        public void DisableInteractive()
        {
            Debug.Log("Disable interactive");
            SetInteractive(false);
        }

        public void EnableInteractive()
        {
            SetInteractive(true);
        }

        private void SetInteractive(bool enable)
        {
            if (isInteractive == enable) return;

            isInteractive = enable;

            foreach (var cube in cubesGrid)
            {
                cube.GetComponent<Rigidbody>().isKinematic = !enable;
                cube.GetComponent<CubeMovement>().enabled = !enable;
            }
        }
    }
}