using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;


namespace HairSalon.Models
{
public class Stylist
{
private string _name;
private int _id;


public Stylist (string name,  int id=0)
{
	_name = name;
	_id = id;
}

public override bool Equals (System.Object otherStylist)
{
	if(!(otherStylist is Stylist))
	{
		return false;
	}
	else
	{
		Stylist newStylist = (Stylist) otherStylist;
		bool idEquality = this.GetId().Equals(newStylist.GetId());
		bool nameEquality = this.GetName().Equals(newStylist.GetName());
		return (idEquality && nameEquality);
	}
}

public override int GetHashCode()
{
	return this.GetId().GetHashCode();
}

public string GetName()
{
	return _name;
}
public int GetId()
{
	return _id;
}
public void Save()
{
	MySqlConnection conn = DB.Connection();
	conn.Open();

	var cmd = conn.CreateCommand() as MySqlCommand;
	cmd.CommandText = @"INSERT INTO stylists (name) VALUES (@name);";

	MySqlParameter name = new MySqlParameter();
	name.ParameterName = "@name";
	name.Value = this._name;
	cmd.Parameters.Add(name);

	cmd.ExecuteNonQuery();
	_id = (int) cmd.LastInsertedId;
	conn.Close();
	if (conn != null)
	{
		conn.Dispose();
	}
}

public static Stylist Find(int id)
{
	MySqlConnection conn = DB.Connection();
	conn.Open();
	var cmd = conn.CreateCommand() as MySqlCommand;
	cmd.CommandText = @"SELECT * FROM stylists WHERE id = (@searchId);";

	MySqlParameter searchId = new MySqlParameter();
	searchId.ParameterName = "@searchId";
	searchId.Value = id;
	cmd.Parameters.Add(searchId);

	var rdr = cmd.ExecuteReader() as MySqlDataReader;
	int StylistId = 0;
	string StylistName = "";

	while(rdr.Read())
	{
		StylistId = rdr.GetInt32(0);
		StylistName = rdr.GetString(1);
	}
	Stylist newStylist = new Stylist(StylistName, StylistId);
	conn.Close();
	if (conn != null)
	{
		conn.Dispose();
	}
	return newStylist;
}

public static List<Stylist> GetAll()
{
	List<Stylist> allStylists = new List<Stylist> {
	};
	MySqlConnection conn = DB.Connection();
	conn.Open();
	var cmd = conn.CreateCommand() as MySqlCommand;
	cmd.CommandText = @"SELECT * FROM stylists;";
	var rdr = cmd.ExecuteReader() as MySqlDataReader;
	while(rdr.Read())
	{
		int StylistId = rdr.GetInt32(0);
		string StylistName = rdr.GetString(1);
		Stylist newStylist = new Stylist(StylistName, StylistId);
		allStylists.Add(newStylist);
	}
	conn.Close();
	if (conn != null)
	{
		conn.Dispose();
	}
	return allStylists;
}

public List<Client> GetClients()
{
	List<Client> allStylistClients = new List<Client> {
	};
	MySqlConnection conn = DB.Connection();
	conn.Open();
	MySqlCommand cmd = conn.CreateCommand();
	cmd.CommandText = @"SELECT clients. *
										FROM stylists
										JOIN stylists_clients ON (stylists.id = stylists_clients.stylist_id)
										JOIN clients ON (clients.id = stylists_clients.client_id)
										WHERE stylists.id = @StylistId;";
	MySqlParameter stylistIdParameter = new MySqlParameter();
	stylistIdParameter.ParameterName = "@StylistId";
	stylistIdParameter.Value = this._id;
	cmd.Parameters.Add(stylistIdParameter);
	MySqlDataReader rdr = cmd.ExecuteReader();
	while(rdr.Read())
	{
		int clientId = rdr.GetInt32(0);
		string clientName = rdr.GetString(1);
		DateTime clientAppointment = rdr.GetDateTime(2);

		Client newClient = new Client(clientName, clientAppointment, clientId);
		allStylistClients.Add(newClient);
	}
	conn.Close();
	if (conn != null)
	{
		conn.Dispose();
	}
	return allStylistClients;
}

public List<Speciality> GetSpecialities()
{
	List<Speciality> allStylistSpecialities = new List<Speciality> {
	};
	MySqlConnection conn = DB.Connection();
	conn.Open();
	MySqlCommand cmd = conn.CreateCommand();
	cmd.CommandText = @"SELECT specialities. *
										FROM stylists
										JOIN specialities_stylists ON (stylists.id = specialities_stylists.stylist_id)
										JOIN specialities ON (specialities.id = specialities_stylists.speciality_id)
										WHERE stylists.id = @StylistId;";
	MySqlParameter stylistIdParameter = new MySqlParameter("@StylistId",this._id);
	cmd.Parameters.Add(stylistIdParameter);
	MySqlDataReader rdr = cmd.ExecuteReader();
	while(rdr.Read())
	{
		int specialityId = rdr.GetInt32(0);
		string specialityName = rdr.GetString(1);

		Speciality newSpeciality = new Speciality(specialityName, specialityId);
		allStylistSpecialities.Add(newSpeciality);
	}
	conn.Close();
	if (conn != null) conn.Dispose();

	return allStylistSpecialities;
}

public static void ClearAll()
{
	MySqlConnection conn = DB.Connection();
	conn.Open();
	var deleteClients = conn.CreateCommand() as MySqlCommand;
	deleteClients.CommandText = @"DELETE FROM clients;";
	deleteClients.ExecuteNonQuery();

	var cmd = conn.CreateCommand() as MySqlCommand;
	cmd.CommandText = @"DELETE FROM stylists;";
	cmd.ExecuteNonQuery();

	conn.Close();
	if(conn != null)
	{
		conn.Dispose();
	}
}

public void Edit(string newName)
{
	MySqlConnection conn = DB.Connection();
	conn.Open();
	var cmd = conn.CreateCommand() as MySqlCommand;
	cmd.CommandText = @"UPDATE stylists SET name = @newName WHERE id = @searchId;";
	MySqlParameter searchId = new MySqlParameter();
	searchId.ParameterName = "@searchId";
	searchId.Value = _id;
	cmd.Parameters.Add(searchId);
	MySqlParameter name = new MySqlParameter();
	name.ParameterName = "@newName";
	name.Value = newName;
	cmd.Parameters.Add(name);
	cmd.ExecuteNonQuery();
	_name = newName;
	conn.Close();
	if (conn != null)
	{
		conn.Dispose();
	}
}

public void Delete()
{
	MySqlConnection conn = DB.Connection();
	conn.Open();
	MySqlCommand cmd = conn.CreateCommand();
	cmd.CommandText = @"DELETE FROM stylists WHERE id=@StylistId; DELETE FROM stylists_clients WHERE stylist_id = @StylistId;";
	MySqlParameter stylistParameter = new MySqlParameter();
	stylistParameter.ParameterName = "@StylistId";
	stylistParameter.Value = this.GetId();
	cmd.Parameters.Add(stylistParameter);
	cmd.ExecuteNonQuery();
	conn.Close();
	if (conn != null)
	{
		conn.Dispose();
	}
}


public void AddClient (Client client)
{
	MySqlConnection conn = DB.Connection();
	conn.Open();
	MySqlCommand cmd = conn.CreateCommand();
	cmd.CommandText = @"INSERT INTO stylists_clients (client_id, stylist_id) VALUES (@ClientId, @StylistId);";
	MySqlParameter clientId = new MySqlParameter("@ClientId", client.GetId());
	MySqlParameter stylistId = new MySqlParameter("@StylistId",this._id);
	cmd.Parameters.Add(clientId);
	cmd.Parameters.Add(stylistId);

	cmd.ExecuteNonQuery();

	conn.Close();
	if (conn != null) conn.Dispose();
}

public void AddSpeciality (Speciality speciality)
{
	MySqlConnection conn = DB.Connection();
	conn.Open();
	MySqlCommand cmd = conn.CreateCommand();
	cmd.CommandText = @"INSERT INTO specialities_stylists (stylist_id, speciality_id) VALUES (@StylistId, @SpecialityId);";
	MySqlParameter specialityId = new MySqlParameter("@SpecialityId", speciality.GetId());
	MySqlParameter stylistId = new MySqlParameter("@StylistId",this._id);
	cmd.Parameters.Add(specialityId);
	cmd.Parameters.Add(stylistId);

	cmd.ExecuteNonQuery();

	conn.Close();
	if (conn != null) conn.Dispose();
}

}
}
