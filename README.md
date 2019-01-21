# VkNet.TokenMagic
### Languages: [EN](#Readme-English), [RU](#Readme-Русский)

# Readme English

Extension for [VkNet](https://github.com/vknet/vk) to authenticate as Android application, giving you access to full Audio API and probably more.
This is a heavily refactored C# port of [PHP vk-audio-token](https://github.com/vodka2/vk-audio-token) - all credits for reverse engineering and black magic to original author.

How it works:
* Makes requests to Google servers as an Android app -> gets `receipt` string
* Authenticates in VK using regular flow with login and password -> gets `token` string
* Authenticates in VK arain using `receipt` and `token` -> gets **`refreshed token`**

### This `refreshed token` gives you god mode and allows full API access.

## Installation

### Nuget
    Install-Package VkNet.TokenMagic
    
### .NET CLI
    dotnet add package VkNet.TokenMagic

## Examples
```c#
    //Add all the stuff to DI container:
    var services = new ServiceCollection();
    services.AddVkTokenMagic();
    var vkNet = new VkApi(services);
    
    // use login+password
    vkNet.Authorize(new ApiAuthParams
    {
        Login = "LOGIN",
        Password = "PASSWORD",
    });
```
[Full test project is here](https://github.com/Rast1234/VkNet.TokenMagic/blob/master/Example/Program.cs)

## Notes
* Cache token if possible!
* Authentication flow might be broken in non-trivial cases (2FA, captcha, etc)
* Had to reinvent RestClient with underlying HttpClient
* No dependencies on ProtocolBuffers
* No dependencies on external services except Gooogle


# Readme Русский

Расширение [VkNet](https://github.com/vknet/vk) для аутентификации под видом Android-приложения для доступа к полному API аудиозаписей и, возможно, даже больше.
Это сильно отрефакторенный порт [PHP vk-audio-token](https://github.com/vodka2/vk-audio-token) - вся уважуха за реверс и черную магию автору оригинала.

Как оно работает:
* Делает запросы к серверам Google, прикидываясь Android-приложением -> получает строку `receipt`
* Аутентифицируется в VK по стандартной схеме с логином и паролем -> получает строку `токен`
* Аутентифицируется в VK еще раз, используя `receipt` и `токен` -> получает строку **`обновленный токен`**

### Этот `обновленный токен` дает расширенные права и полный доступ к API.


## Установка

### Nuget
    Install-Package VkNet.TokenMagic
    
### .NET CLI
    dotnet add package VkNet.TokenMagic

## Примеры
```c#
    //Добавляем все в DI контейнер:
    var services = new ServiceCollection();
    services.AddVkTokenMagic();
    var vkNet = new VkApi(services);
    
    // используем логин+пароль
    vkNet.Authorize(new ApiAuthParams
    {
        Login = "LOGIN",
        Password = "PASSWORD",
    });
```
[Полный тестовый проект здесь](https://github.com/Rast1234/VkNet.TokenMagic/blob/master/Example/Program.cs)

## Заметки
* Кэшируйте токен, если возможно!
* Аутентификация может быть сломана в нетривиальных случаях (2-х факторная, капча, и тд)
* Пришлось переизобретать RestClient с используемым HttpClient
* Нет зависимости от ProtocolBuffers
* Нет зависимости от внешних сервисов, помимо Google
