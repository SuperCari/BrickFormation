﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcadeGridP4 : MonoBehaviour
{
    public static int gridP4Width = 10;
    public static int gridP4Height = 20;
    public static Transform[,] gridP4 = new Transform[gridP4Width, gridP4Height];
    public GameObject[] bricks;
    public GameObject player;
    private int[] rng;
    private int rngIndex = 0;

    void Start() {
        rng = FindObjectOfType<UtilityRNG>().randomNumbers.ToArray();
        instantiateNextBrick();
    }

    // check if a brick is within the grid's boundaries
    public bool isInsideGrid(Vector2 pos) {
        return ((int)pos.x >= 0 && (int)pos.x < gridP4Width && (int)pos.y >= 0);
    }

    // update the grid's bricks: new bricks are always resetted until they land --> become part of the grid
    public void updateGrid(GameObject brick) {
        for (int y = 0; y < gridP4Height; y++)                                        // check every row                                    
            for (int x = 0; x < gridP4Width; x++)                                     // and every column
                if (gridP4[x, y] != null && gridP4[x, y].parent == brick.transform)     // check if there is a brick
                    gridP4[x, y] = null;                                              // act like it's not there
        foreach (Transform cube in brick.transform) {
            Vector2 pos = new Vector2(Mathf.Round(cube.position.x), Mathf.Round(cube.position.y));
            if (pos.y < gridP4Height) gridP4[(int)pos.x, (int)pos.y] = cube;               // place a brick into the grid
        }
    }

    // return a transform in the grid at a certain position; used for checking valid moves
    public Transform getTransformAtGridPosition(Vector2 pos) {
        if (pos.y > gridP4Height - 1) return null;
        else return gridP4[(int)pos.x, (int)pos.y];
    }

    // everytime a brick lands, check if the row is full and if it is, clear it and adjust the others
    public void updateRows() {
        for (int y = 0; y < gridP4Height; y++) {
            if (isRowFullAtY(y)) {
                deleteRowAtY(y);
                moveRowsDown(y + 1);
                y--;
            }
        }
    }

    public bool isRowFullAtY(int y) {
        for (int x = 0; x < gridP4Width; x++) if (gridP4[x, y] == null) return false;
        return true;
    }

    public void deleteRowAtY(int y) {
        for (int x = 0; x < gridP4Width; x++) {
            Destroy(gridP4[x, y].gameObject);
            gridP4[x, y] = null;
        }
    }

    public void moveRowsDown(int y) {
        for (int i = y; i < gridP4Height; i++) moveRowDown(i);
    }

    public void moveRowDown(int y) {
        for (int x = 0; x < gridP4Width; x++) {
            if (gridP4[x, y] != null) {
                gridP4[x, y - 1] = gridP4[x, y];
                gridP4[x, y] = null;
                gridP4[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    // check the highest row for cubes
    public void checkGameOver(GameObject brick) {
        for (int x = 0; x < gridP4Width; x++) {
            foreach (Transform cube in brick.transform) {
                Vector2 pos = new Vector2((int)Mathf.Round(cube.position.x), (int)Mathf.Round(cube.position.y));
                if (pos.y > gridP4Height - 1) {
                    PlayerPrefs.SetInt("gameover", 1);
                    break;
                }
            }
        }
    }

    public void instantiateNextBrick() {
        int random = rng[rngIndex];
        GameObject nextBrick = (GameObject)Instantiate(bricks[random], player.transform);
        nextBrick.transform.position += new Vector3(5, 20, 0);
        if (rngIndex == 63) rngIndex = 0; else rngIndex++;
    }
}