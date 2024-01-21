using UnityEngine;
using System.Data.SQLite;
using UnityEngine.SceneManagement;
using System.Data;
using System.Collections.Generic;

public class DBConnector : MonoBehaviour
{
    [SerializeField] string name;
    [SerializeField] int id;
    [SerializeField] float x;
    [SerializeField] float y;
    [SerializeField] float z;
    [SerializeField] int score;
    [SerializeField] int hp = 100;

    GameObject _playerObject;
    GameObject _scoreManager;
    PointsManager _pointsManager;
    PlaceHolderManager _placeHolderManager;
    bool newGame = false;
    void Start()
    {
        //SQLiteConnection connection =
        //                 new SQLiteConnection(@"Data Source=D:\mydb.db;Version=3;");
        //stores in location C:\Users\@username@\AppData\LocalLow\DefaultCompany\City Stories
        SQLiteConnection connection =
                         new SQLiteConnection("Data Source=" + Application.persistentDataPath + @"/" + "My_Database.db;Version=3;");

        connection.Open();
        SQLiteCommand command = connection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = "CREATE TABLE IF NOT EXISTS 'highscores' ( " +
                          "  'id' INTEGER PRIMARY KEY, " +
                          "  'name' TEXT NOT NULL, " +
                          "  'score' INTEGER NOT NULL," +
                          "  'hp' INTEGER NOT NULL" +
                          ");";
        command.ExecuteNonQuery();
        command.CommandText = "CREATE TABLE IF NOT EXISTS 'location' ( " +
                          "  'id' INTEGER PRIMARY KEY, " +
                          "  'locX' FLOAT NOT NULL," +
                          "  'locY' FLOAT NOT NULL," +
                          "  'locZ' FLOAT NOT NULL" +
                          ");";
        command.ExecuteNonQuery();
        connection.Close();

        //=================================================
        /*
         string connection = "URI=file:" + Application.persistentDataPath + "/" + "My_Database";
		
         */
        //=================================================
    }

    public void SaveDataToDB()
    {
        // check if name already exists in db if so override data
        id = SelectID(name);
        if (id == -1)
        {
            id = DefineNewLastId();
            AddParamToDB();
            //insert
        }
        else
        {
            UpdateParamDB();
            //update
        }
        // save data in new id if name is unique
    }

    public void LoadDataFromDB()
    {
        try
        {
            id = SelectID(name);
            // search for id by name
            //try to load data
        }
        catch (System.Exception e)
        {
            Debug.Log("An error occured " + e.Message);
            throw;
        }
        if (id == -1)
        {
            _placeHolderManager.NameNotFound();
            Debug.Log(name + " not found in data base, couldn't load data");
        }
        else
        {
            // load data for selected id
            LoadParamsById(id);
            newGame = true;
            SceneManager.LoadScene("MainGame");

            if (_playerObject != null)
            {
                _playerObject.transform.position = new Vector3(x, y, z);
                _pointsManager.UpdatePoints(score);
                _playerObject.SetActive(true);
            }
            // find player object
            // set parameters after load
        }
    }
    private void OnLevelWasLoaded(int level)
    {
        if (level == 0) // main menu level 0
        {
            SetPosition(new Vector3(0, 1.08f, 0));
            SetScore(0);
        }
        if (level != 1)
        {
            return;
        }
        _playerObject = GameObject.Find("Player");
        _scoreManager = GameObject.Find("ActualPoints");
        _pointsManager = _scoreManager.GetComponent<PointsManager>() as PointsManager;
        if (newGame)
        {
            if (_playerObject != null)
            {
                _playerObject.transform.position = new Vector3(x, y, z);
                _pointsManager.SetPointsOnGameLoad(score);
                _playerObject.SetActive(true);
            }
        }
    }

    private static DBConnector dbconn;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (dbconn == null)
            dbconn = this;
        else
            Object.Destroy(gameObject);
    }

    public void SetName(string _name)
    {
        this.name = _name;
    }

    public void SetScore(int _score)
    {
        this.score = _score;
    }

    public void SetPosition(Vector3 _pos)
    {
        this.x = _pos.x;
        this.y = _pos.y;
        this.z = _pos.z;
    }

    private int SelectID(string _name)
    {
        int result = -1;

        SQLiteConnection connection =
                         new SQLiteConnection("Data Source=" + Application.persistentDataPath + @"/" + "My_Database.db;Version=3;");

        connection.Open();
        SQLiteCommand command = connection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;

        command.CommandText = "Select id FROM highscores WHERE name = @name;";
        command.Parameters.AddWithValue("@name", _name);

        IDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            result = int.Parse(reader[0].ToString());
        }

        connection.Close();
        return result;
    }

    private int DefineNewLastId()
    {
        int newId = 0;

        SQLiteConnection connection =
                         new SQLiteConnection("Data Source=" + Application.persistentDataPath + @"/" + "My_Database.db;Version=3;");

        connection.Open();
        SQLiteCommand command = connection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;

        command.CommandText = "Select id FROM highscores;";

        IDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            newId = int.Parse(reader[0].ToString());
        }

        connection.Close();

        newId++;

        return newId;
    }

    private void LoadParamsById(int _id)
    {
        SQLiteConnection connection =
                         new SQLiteConnection("Data Source=" + Application.persistentDataPath + @"/" + "My_Database.db;Version=3;");

        connection.Open();
        SQLiteCommand command = connection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;

        command.CommandText = "Select locX, locY, locZ FROM location WHERE id = @id;";
        command.Parameters.AddWithValue("@id", _id);

        IDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            x = float.Parse(reader[0].ToString());
            y = float.Parse(reader[1].ToString());
            z = float.Parse(reader[2].ToString());
            //score = int.Parse(reader[0].ToString());
        }
        connection.Close();

        connection.Open();
        command = connection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;

        command.CommandText = "Select score FROM highscores WHERE id = @id;";
        command.Parameters.AddWithValue("@id", _id);

        reader = command.ExecuteReader();
        while (reader.Read())
        {
            score = int.Parse(reader[0].ToString());
        }

        connection.Close();
    }

    private void AddParamToDB()
    {
        SQLiteConnection connection =
                         new SQLiteConnection("Data Source=" + Application.persistentDataPath + @"/" + "My_Database.db;Version=3;");

        connection.Open();
        SQLiteCommand command = connection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;

        command.CommandText = "INSERT INTO highscores (id, name, score, hp) "
            +"VALUES (@id, @name, @score, @hp);";
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@name", name);
        command.Parameters.AddWithValue("@score", score);
        command.Parameters.AddWithValue("@hp", hp);
        int i = command.ExecuteNonQuery();
        connection.Close();

        connection.Open();
        command = connection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;

        command.CommandText = "INSERT INTO location (id, locX, locY, locZ) "
            + "VALUES (@id, @locX, @locY, @locZ);";
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@locX", x);
        command.Parameters.AddWithValue("@locY", y);
        command.Parameters.AddWithValue("@locZ", z);
        i = command.ExecuteNonQuery();
        connection.Close();
    }

    private void UpdateParamDB()
    {
        SQLiteConnection connection =
                         new SQLiteConnection("Data Source=" + Application.persistentDataPath + @"/" + "My_Database.db;Version=3;");

        connection.Open();
        SQLiteCommand command = connection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;

        command.CommandText = "UPDATE highscores SET name = @name, score = @score, hp = @hp WHERE id = @id";
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@name", name);
        command.Parameters.AddWithValue("@score", score);
        command.Parameters.AddWithValue("@hp", hp);
        int i = command.ExecuteNonQuery();
        connection.Close();

        connection.Open();
        command = connection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;

        command.CommandText = "UPDATE location SET locX = @locX, locY = @locY, locZ = @locZ WHERE id = @id";
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@locX", x);
        command.Parameters.AddWithValue("@locY", y);
        command.Parameters.AddWithValue("@locZ", z);
        int j = command.ExecuteNonQuery();
        connection.Close();
    }

    public void PassPlaceHoldManager(PlaceHolderManager _plHolMan)
    {
        _placeHolderManager = _plHolMan;
    }

    private List<HighscorePlayer> LoadHighscores()
    {
        List<HighscorePlayer> result = new List<HighscorePlayer>();

        SQLiteConnection connection =
                         new SQLiteConnection("Data Source=" + Application.persistentDataPath + @"/" + "My_Database.db;Version=3;");

        connection.Open();
        SQLiteCommand command = connection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;

        command.CommandText = "Select name, score FROM highscores;";

        IDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            HighscorePlayer playerScore = new HighscorePlayer();
            playerScore.SetName(reader[0].ToString());
            playerScore.SetPoints(int.Parse(reader[1].ToString()));
            result.Add(playerScore);
        }

        connection.Close();

        return result;
    }

    public List<HighscorePlayer> GetHighscores()
    {
        List<HighscorePlayer> result = LoadHighscores();
        return result;
    }
}