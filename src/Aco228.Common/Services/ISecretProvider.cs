namespace Aco228.Common.Services;

public interface ISecretProvider
{
    string Get(string key);
}