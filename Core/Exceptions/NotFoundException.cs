namespace Core.Exceptions;

public class NotFoundException(string from, string to) : Exception($"A currency from {from} to {to} was not found");