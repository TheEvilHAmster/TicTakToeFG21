using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Microsoft.Win32;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace FG {

    public class Board : MonoBehaviour {
        [SerializeField] private GameObject _tilePrefab;
        public Player playerOne;
        public Player playerTwo;

        [Header("Events")] public PlayerEvent switchPlayerEvent;

        public UnityEvent didPlaceEvent;

        private int _slotsToWin;
        private int _boardSize;
        private Tile[,] _tiles;
        private GamePiece[,] _pieces;
        private List<Vector2Int> winningTiles;

        private Transform _tilesTransform;
        private Transform _piecesTransform;
        
        private const float _timeBetweenMarkingWinningTiles = 0.5f;
        private const float _timeToFadeWinningTiles = 0.5f;
        public Player CurrentPlayer { get; private set; }

        public Tile this[int row, int column] => _tiles[row, column];

        public bool PlaceMarkerOnTile(Tile tile) {
            if (ReferenceEquals(CurrentPlayer, null)) {
                return false;
            }
            
            if (ReferenceEquals(_pieces[tile.gridPosition.x, tile.gridPosition.y], null)) {
                GamePiece piece = Instantiate(CurrentPlayer.piecePrefab,
                    new Vector3(tile.gridPosition.x, -tile.gridPosition.y),
                    Quaternion.identity, _piecesTransform)?.GetComponent<GamePiece>();
                if (!ReferenceEquals(piece, null)) {
                    piece.Owner = CurrentPlayer;
                    _pieces[tile.gridPosition.x, tile.gridPosition.y] = piece;
                }

                didPlaceEvent.Invoke();

                if (_slotsToWin <= IsThereAWinner(tile))
                {
                    Debug.Log("winner");
                }
                
                SwitchPlayer();
                return true;
            }
            
            
            return false;
        }

        int IsThereAWinner(Tile tile)
        {

            if (_slotsToWin <= XLookingIfWinning(tile))
            {
                Debug.Log($"x dir {CurrentPlayer} won");
            }
                
            if (_slotsToWin <= YLookingIfWinning(tile))
            {
                Debug.Log($"y dir {CurrentPlayer} won");
            }
                
            if (_slotsToWin <= XYPossitivIfWinning(tile))
            {
                Debug.Log($"xy pos dir {CurrentPlayer} won");
            }
            if (_slotsToWin <= XYNeggativIfWinning(tile))
            {
                Debug.Log($"xy neg dir {CurrentPlayer} won");
            }
                
            return 0;
        }

        #region For loop try to solve tic tac toe

        

        
        int YLookingIfWinning(Tile tile)
        {
            try
            {

                int inARow = 0;
                int currentlyInARow = 0;
                List<Vector2Int> tempWinningTiles = new List<Vector2Int>();
                

                // checks tiles in x if they are the same as the pressed tile
                for (int i = 0; i < _boardSize; i++)
                {
                    if (_pieces[tile.gridPosition.x, i] == null)
                    {
                        currentlyInARow = 0;
                        continue;
                    }
                    if (_pieces[tile.gridPosition.x, i].Owner == _pieces[tile.gridPosition.x, tile.gridPosition.y].Owner)
                    {
                        currentlyInARow++;
                        tempWinningTiles.Add(new Vector2Int(tile.gridPosition.x, i));
                    }
                    else
                    {
                        currentlyInARow = 0;
                    }

                    if (inARow < currentlyInARow)
                    {
                        inARow = currentlyInARow;
                    }
                    if (inARow == _slotsToWin)
                    {
                        winningTiles = tempWinningTiles;
                        return inARow;
                    }
                }
                return inARow;
            }
            catch (Exception e)
            {
                Debug.Log($"y faccin error{e}");
            }

            return 0;
        }

        int XLookingIfWinning(Tile tile)
        {
            try
            {
                int inARow = 0;
                int currentlyInARow = 0;
                List<Vector2Int> tempWinningTiles = new List<Vector2Int>();

                // checks tiles in x if they are the same as the pressed tile

                for (int i = 0; i < _boardSize; i++)
                {
                    if (_pieces[i, tile.gridPosition.y] == null)
                    {
                        currentlyInARow = 0;
                        continue;
                    }
                    if (_pieces[i, tile.gridPosition.y].Owner == _pieces[tile.gridPosition.x, tile.gridPosition.y].Owner)
                    {
                        currentlyInARow++;
                        tempWinningTiles.Add(new Vector2Int(i, tile.gridPosition.y));
                    }
                    else
                    {
                        currentlyInARow = 0;
                    }

                    if (inARow < currentlyInARow)
                    {
                        inARow = currentlyInARow;
                    }

                    if (inARow == _slotsToWin)
                    {
                        winningTiles = tempWinningTiles;
                        return inARow;
                    }
                    
                }
                return inARow;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Debug.Log($"x faccin error {e}");
            }

            return 0;
        }

        int XYPossitivIfWinning(Tile tile)
        {
            try
            {
                int inARow = 0;
                int currentlyInARow = 0;
                bool goingToTheCorner = true;
                List<Vector2Int> tempWinningTiles = new List<Vector2Int>();
                int x;
                int y;


                //moves the x,y to the edge
                    x = tile.gridPosition.x;
                    y = tile.gridPosition.y;
                    for (int i = 0; i < _boardSize; i++)
                    {

                        if (x > 0 && y > 0)
                        {
                            x--;
                            y--;
                        }
                        else
                        {
                            goingToTheCorner = false;
                            break;
                        }
                    }
                    
                    // checks tiles in x y positiv if they are the same as the pressed tile

                if (!goingToTheCorner)
                {
                    for (int i = 0; i < _boardSize; i++)
                    {
                        if (_pieces[x, y] == null)
                        {
                            currentlyInARow = 0;
                            if (x == _boardSize - 1 || y == _boardSize - 1)
                            {
                                continue;
                            }
                            x++;
                            y++;
                            continue;
                        }
                        if (_pieces[x, y].Owner == _pieces[tile.gridPosition.x, tile.gridPosition.y].Owner)
                        {
                            
                            currentlyInARow++;
                            tempWinningTiles.Add(new Vector2Int(x,y));

                        }
                        else
                        {
                            currentlyInARow = 0;
                            x++;
                            y++;
                        }

                        if (inARow < currentlyInARow)
                        {
                            inARow = currentlyInARow;
                        }
                        if (inARow == _slotsToWin)
                        {
                            winningTiles = tempWinningTiles;
                            return inARow;
                        }

                        if (x == _boardSize-1 || y == _boardSize-1)
                        {
                            return inARow;
                        }
                        x++;
                        y++;
                    }
                }

                
                return inARow;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Debug.Log($"xy faccin error {e}");
            }
            
            return 0;
        }

        int XYNeggativIfWinning(Tile tile)
        {

            try
            {
                int inARow = 0;
                int currentlyInARow = 0;
                bool goingToTheCorner = true;
                int x;
                int y;
                List<Vector2Int> tempWinningTiles = new List<Vector2Int>();


                //moves the x,y to the edge
                    x = tile.gridPosition.x;
                    y = tile.gridPosition.y;
                    for (int i = 0; i < _boardSize; i++)
                    {

                        if (x > 0 && y < _boardSize-1)
                        {
                            x--;
                            y++;
                        }
                        else
                        {
                            goingToTheCorner = false;
                            break;
                        }
                    }
                    
                    // checks tiles in x y positiv if they are the same as the pressed tile

                if (!goingToTheCorner)
                {
                    for (int i = 0; i < _boardSize; i++)
                    {
                        if (_pieces[x, y] == null)
                        {
                            currentlyInARow = 0;
                            if (x == _boardSize - 1 || y == 0)
                            {
                                continue;
                            }
                            x++;
                            y--;
                            continue;
                        }
                        if (_pieces[x, y].Owner == _pieces[tile.gridPosition.x, tile.gridPosition.y].Owner)
                        {
                            currentlyInARow++;
                            tempWinningTiles.Add(new Vector2Int(x,y));
                        }
                        else
                        {
                            currentlyInARow = 0;
                            x++;
                            y--;
                        }

                        if (inARow < currentlyInARow)
                        {
                            inARow = currentlyInARow;
                        }
                        if (inARow == _slotsToWin)
                        {
                            winningTiles = tempWinningTiles;
                            return inARow;
                        }

                        if (x == _boardSize-1 || y == 0)
                        {
                            
                            return inARow;
                        }
                        x++;
                        y--;
                    }
                }

                
                return inARow;
            }
            catch (Exception e)
            {
                Debug.Log($"xy negativ faccin error{e}");
            }
            
            return 0;

        }
        

        #endregion



        private IEnumerator MarkWinningTiles(List<Vector2Int> winningTiles, Color color) {
            foreach (Vector2Int tile in winningTiles) {
                StartCoroutine(FadeTile(_tiles[tile.x, tile.y], color));
                yield return new WaitForSeconds(_timeBetweenMarkingWinningTiles);
            }
        }

        private IEnumerator FadeTile(Tile tile, Color targetColor) {
            SpriteRenderer tileRenderer = tile.GetComponent<SpriteRenderer>();
            float elapsedTime = 0f;
            Color startColor = tileRenderer.color;
            float fadeTime = _timeToFadeWinningTiles;
            
            while (elapsedTime < fadeTime) {
                elapsedTime += Time.deltaTime;
                float blend = Mathf.Clamp01(elapsedTime / fadeTime);
                tileRenderer.color = Color.Lerp(startColor, targetColor, blend);
                yield return null;
            }

            tileRenderer.color = targetColor;
        }

        private void SwitchPlayer() {
            CurrentPlayer = ReferenceEquals(CurrentPlayer, playerOne) ? playerTwo : playerOne;
            switchPlayerEvent.Invoke(CurrentPlayer);
        }

        private void SetupTiles() {
            for (int x = 0; x < _boardSize; x++) {
                for (int y = 0; y < _boardSize; y++) {
                    GameObject tileGo = Instantiate(_tilePrefab, new Vector3(x, -y, 0f), Quaternion.identity,
                        _tilesTransform);
                    tileGo.name = $"Tile_({x},{y})";

                    Tile tile = tileGo.GetComponent<Tile>();
                    tile.board = this;
                    tile.gridPosition = new Vector2Int(x, y);

                    _tiles[x, y] = tile;
                }
            }
        }

        private void SetCurrentPlayer() {
            CurrentPlayer = Random.Range(0, 2) == 0 ? playerOne : playerTwo;
            switchPlayerEvent.Invoke(CurrentPlayer);
        }

        public void Awake() {
            _tilesTransform = transform.GetChild(0);
            _piecesTransform = transform.GetChild(1);
            _boardSize = PlaySettings.BoardSize;
            _slotsToWin = PlaySettings.SlotsToWin;
            winningTiles = new List<Vector2Int>();

            _tiles = new Tile[_boardSize, _boardSize];
            _pieces = new GamePiece[_boardSize, _boardSize];

            SetupTiles();

            playerOne.displayName = PlaySettings.PlayerOneName;
            playerTwo.displayName = PlaySettings.PlayerTwoName;

            SetCurrentPlayer();
        }
    }
}