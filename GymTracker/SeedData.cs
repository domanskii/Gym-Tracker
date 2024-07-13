using Microsoft.AspNetCore.Identity;

public class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string roleName = "Admin";
            IdentityResult roleResult;

            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            IdentityUser admin = await userManager.FindByEmailAsync("admin@admin.com");

            if (admin == null)
            {
                IdentityUser user = new IdentityUser
                {
                    UserName = "admin@admin.com",
                    Email = "admin@admin.com",
                    EmailConfirmed = true
                };
                
                IdentityResult result = await userManager.CreateAsync(user, "Admin123$");

                if (result.Succeeded)
                {
                    await userManager.UpdateAsync(user);
                    var addToRoleResult = await userManager.AddToRoleAsync(user, roleName);
                    Console.WriteLine($"Add to role result: {addToRoleResult.Succeeded}");
                }
            }
        }
    }
}