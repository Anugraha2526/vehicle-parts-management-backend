namespace vehicle_parts_management_backend.Application.DTOs.StaffDTOs
{
    // data sent back to the client after staff operations
    public class StaffResponseDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // role shown as a readable string like "Admin" or "Staff"
        public string Role { get; set; } = string.Empty;

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
