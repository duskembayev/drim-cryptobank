using cryptobank.api.features.accounts.domain;
using cryptobank.api.features.news.domain;
using cryptobank.api.features.users.domain;

namespace cryptobank.api.db;

internal static class PredefinedDataSeed
{
    public static async Task ApplyReferencesAsync(this CryptoBankDbContext dbContext)
    {
        dbContext.Roles.Add(Role.Detached.User);
        dbContext.Roles.Add(Role.Detached.Analyst);
        dbContext.Roles.Add(Role.Detached.Administrator);

        await dbContext.SaveChangesAsync();
    }

    public static async Task ApplySamplesAsync(this CryptoBankDbContext dbContext)
    {
        dbContext.News.Add(new News
        {
            Id = "1EE9D164-883D-4E05-AF8D-1A541A6E5D92",
            Title = "Bitcoin is going to the moon!",
            Content = "Bitcoin is going to the moon! Our analysts predict a price of $100,000 by the end of the year!",
            Date = new DateTime(2021, 10, 1, 15, 45, 0, DateTimeKind.Utc),
            Author = "John Doe"
        });

        dbContext.News.Add(new News
        {
            Id = "80B4F2EA-A9A0-418B-B158-6816CDE7E6CD",
            Title = "Oh no, Bitcoin is crashing!",
            Content = "Oh no, Bitcoin is crashing! This is the end of the world!",
            Date = new DateTime(2022, 3, 5, 18, 5, 0, DateTimeKind.Utc),
            Author = "Jane Doe"
        });

        dbContext.Users
            .Add(new User
            {
                Email = "bender@futurama.com",
                DateOfRegistration = new DateTime(2019, 4, 13, 11, 28, 16, DateTimeKind.Utc),
                DateOfBirth = new DateOnly(1996, 9, 4),
                Roles = { Role.Detached.User },
                PasswordHash =
                    "$argon2id$v=1$m=8162,t=40,p=8$f1DHhWRqfmwpbs8DbfJrlbYLwdrXXUQu/35DBvwVnOSEbkOdsNBLHEzwKd6lNCamlQsklnrKlIb69LN1EQV31Q==$Kdq41XSqA1C8RThSbWCZZ2ZuOI66S/Yvos1LZSOrul7Bdi7n2wJEaKF69Q+CW7MJKR3wcU3oAlwyiiHOXMU25bmEDakYy4IN7xU7DYYZlNiN0WHKH6L6mwNjED71dTfgq3ORIkL5wT1guoZzyDCtF9VmTYf2yffLg2HZaKvC7Ds=",
            });

        dbContext.Users
            .Add(new User
            {
                Email = "leela@futurama.com",
                DateOfRegistration = new DateTime(2019, 4, 13, 11, 28, 16, DateTimeKind.Utc),
                DateOfBirth = new DateOnly(1996, 9, 4),
                Roles = { Role.Detached.User },
                PasswordHash =
                    "$argon2id$v=1$m=8162,t=40,p=8$f1DHhWRqfmwpbs8DbfJrlbYLwdrXXUQu/35DBvwVnOSEbkOdsNBLHEzwKd6lNCamlQsklnrKlIb69LN1EQV31Q==$Kdq41XSqA1C8RThSbWCZZ2ZuOI66S/Yvos1LZSOrul7Bdi7n2wJEaKF69Q+CW7MJKR3wcU3oAlwyiiHOXMU25bmEDakYy4IN7xU7DYYZlNiN0WHKH6L6mwNjED71dTfgq3ORIkL5wT1guoZzyDCtF9VmTYf2yffLg2HZaKvC7Ds=",
                Accounts =
                {
                    new Account
                    {
                        AccountId = "g-leela-1",
                        DateOfOpening = new DateTime(2020, 1, 5, 10, 11, 12, DateTimeKind.Utc)
                    },
                    new Account
                    {
                        AccountId = "g-leela-2",
                        DateOfOpening = new DateTime(2020, 1, 1, 10, 11, 12, DateTimeKind.Utc)
                    }
                }
            });

        dbContext.Users
            .Add(new User
            {
                Email = "fry@futurama.com",
                DateOfRegistration = new DateTime(2019, 4, 13, 11, 28, 16, DateTimeKind.Utc),
                DateOfBirth = new DateOnly(1996, 9, 4),
                Roles = { Role.Detached.User },
                PasswordHash =
                    "$argon2id$v=1$m=8162,t=40,p=8$f1DHhWRqfmwpbs8DbfJrlbYLwdrXXUQu/35DBvwVnOSEbkOdsNBLHEzwKd6lNCamlQsklnrKlIb69LN1EQV31Q==$Kdq41XSqA1C8RThSbWCZZ2ZuOI66S/Yvos1LZSOrul7Bdi7n2wJEaKF69Q+CW7MJKR3wcU3oAlwyiiHOXMU25bmEDakYy4IN7xU7DYYZlNiN0WHKH6L6mwNjED71dTfgq3ORIkL5wT1guoZzyDCtF9VmTYf2yffLg2HZaKvC7Ds=",
                Accounts =
                {
                    new Account
                    {
                        AccountId = "g-fry-1",
                        DateOfOpening = new DateTime(2020, 1, 4, 10, 11, 12, DateTimeKind.Utc)
                    },
                    new Account
                    {
                        AccountId = "g-fry-2",
                        DateOfOpening = new DateTime(2020, 1, 5, 10, 11, 12, DateTimeKind.Utc)
                    }
                }
            });

        dbContext.Users
            .Add(new User
            {
                Email = "zoidberg@futurama.com",
                DateOfRegistration = new DateTime(2019, 4, 13, 11, 28, 16, DateTimeKind.Utc),
                DateOfBirth = new DateOnly(1996, 9, 4),
                Roles = { Role.Detached.User },
                PasswordHash =
                    "$argon2id$v=1$m=8162,t=40,p=8$f1DHhWRqfmwpbs8DbfJrlbYLwdrXXUQu/35DBvwVnOSEbkOdsNBLHEzwKd6lNCamlQsklnrKlIb69LN1EQV31Q==$Kdq41XSqA1C8RThSbWCZZ2ZuOI66S/Yvos1LZSOrul7Bdi7n2wJEaKF69Q+CW7MJKR3wcU3oAlwyiiHOXMU25bmEDakYy4IN7xU7DYYZlNiN0WHKH6L6mwNjED71dTfgq3ORIkL5wT1guoZzyDCtF9VmTYf2yffLg2HZaKvC7Ds=",
                Accounts =
                {
                    new Account
                    {
                        AccountId = "g-zoidberg-1",
                        DateOfOpening = new DateTime(2020, 1, 1, 10, 11, 12, DateTimeKind.Utc)
                    },
                    new Account
                    {
                        AccountId = "g-zoidberg-2",
                        DateOfOpening = new DateTime(2020, 1, 1, 10, 11, 12, DateTimeKind.Utc)
                    }
                }
            });

        await dbContext.SaveChangesAsync();
    }
}