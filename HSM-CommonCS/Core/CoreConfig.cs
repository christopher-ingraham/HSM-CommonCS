namespace HSM_CommonCS.Core
{
    public record DatabaseConfig(string ConnectionString);

    public record RabbitConfig(
            string Host,
            int Port,
            string Username,
            string Password
        );

    public sealed record CoreConfig(
        string ApplicationName,
        DatabaseConfig Database,
        string LogPath,
        RabbitConfig RabbitConfig);

}
