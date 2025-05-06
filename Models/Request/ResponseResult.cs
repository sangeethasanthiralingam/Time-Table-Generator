namespace Time_Table_Generator.Models
{
    public class ResponseResult<TResult>
    {
        public IEnumerable<string>? Errors { get; set; }
        public TResult? Result { get; set; }
        public bool Succeeded { get; set; }

        // Constructor for success
        public ResponseResult(TResult result)
        {
            Result = result;
            Errors = Enumerable.Empty<string>(); // No errors in success
            Succeeded = true;
        }

        // Constructor for failure
        public ResponseResult(IEnumerable<string> errors)
        {
            Errors = errors ?? Enumerable.Empty<string>(); // Ensure errors is not null
            Result = default;
            Succeeded = false;
        }

        // Optional Select method for transforming results if needed
        public ResponseResult<TNew> Select<TNew>(Func<TResult, TNew> selector)
        {
            if (!Succeeded)
                return new ResponseResult<TNew>(Errors!); // Return the errors in case of failure

            var newResult = selector(Result!);
            return new ResponseResult<TNew>(newResult);
        }
    }
}