using Microsoft.AspNetCore.Identity;
using Moq;
using System.Collections.Generic;

public static class MockUserManager
{
    public static Mock<UserManager<TUser>> CreateMock<TUser>() where TUser : class
    {
        var store = new Mock<IUserStore<TUser>>();
        return new Mock<UserManager<TUser>>(
            store.Object, null!, null!, null!, null!, null!, null!, null!, null!
        );
    }
}
