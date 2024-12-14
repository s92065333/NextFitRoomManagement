using Microsoft.AspNetCore.Mvc;
using NextFitRoomManagement.Models;
using System.Collections.Generic;
using System.Linq;

public class RoomAllocationController : Controller
{
    private static List<Room> Rooms = new List<Room>
    {
        new Room { Id = 1, Capacity = 2, IsAllocated = false },
        new Room { Id = 2, Capacity = 4, IsAllocated = false },
        new Room { Id = 3, Capacity = 3, IsAllocated = false },
        new Room { Id = 4, Capacity = 5, IsAllocated = false },
        new Room { Id = 5, Capacity = 6, IsAllocated = false }
    };

    private static int LastAllocatedRoomIndex = 0;

    [HttpGet]
    public IActionResult Index()
    {
        return View(Rooms);
    }

    [HttpPost]
    public IActionResult AllocateRoom(GuestGroup group)
    {
        // Validation: Group size must be greater than 0
        if (group.GroupSize <= 0)
        {
            // Set an error message in TempData
            TempData["StatusMessage"] = "Error: Group size must be greater than 0.";
            return RedirectToAction("Index");
        }

        int startIndex = LastAllocatedRoomIndex;
        int roomCount = Rooms.Count;

        for (int i = 0; i < roomCount; i++)
        {
            int currentIndex = (startIndex + i) % roomCount;
            var room = Rooms[currentIndex];

            if (!room.IsAllocated && room.Capacity >= group.GroupSize)
            {
                room.IsAllocated = true;
                group.AllocationStatus = $"Group allocated to Room {room.Id}";
                LastAllocatedRoomIndex = currentIndex;

                // Set the status message in TempData
                TempData["StatusMessage"] = group.AllocationStatus;
                return RedirectToAction("Index");
            }
        }

        group.AllocationStatus = "No suitable room found for the group.";

        // Set the status message in TempData for "No suitable room"
        TempData["StatusMessage"] = group.AllocationStatus;

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult DeallocateRoom(int roomId)
    {
        var room = Rooms.FirstOrDefault(r => r.Id == roomId);
        if (room != null)
        {
            room.IsAllocated = false;
        }

        // Set deallocation success message in TempData
        TempData["StatusMessage"] = $"Room {roomId} has been deallocated.";

        return RedirectToAction("Index");
    }
}
