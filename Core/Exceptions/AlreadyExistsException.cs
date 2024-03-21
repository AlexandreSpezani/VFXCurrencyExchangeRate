namespace Core.Exceptions;

public class AlreadyExistsException(string message) : Exception($"A currency rate for {message} already exists");