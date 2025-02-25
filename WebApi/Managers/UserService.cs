using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using WebApi.Interfaces;
using WebApi.Models;

namespace WebApi.Managers;
public class UserService : IUserService
{
    private readonly string _filePath = Path.Combine("./", "users.json");
    List<User>? Users { get; }
    int nextId = 3;

    public UserService()
    {
        Users = GetAll();
    }

    public List<User> GetUsers()
    {
        if (!File.Exists(_filePath))
            return [];

        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<User>>(json) ?? [];
    }

    public void SaveUsers(List<User> users)
    {
        var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }

    public List<User>? GetAll() => GetUsers();

    public User? Get(int id)
    {
        return Users?.FirstOrDefault(p => p.Id == id);
    }

    public void Add(User user)
    {
        user.Id = nextId++;
        Users?.Add(user);
    }

    public void Delete(int id)
    {
        var user = Get(id);
        if (user is null)
            return;
        Users?.Remove(user);
    }

    public void Update(User User)
    {
        var index = Users.FindIndex(p => p.Id == User.Id);
        if (index == -1)
            return;
        Users[index] = User;
    }

    public int Count { get => Users.Count; }
}
