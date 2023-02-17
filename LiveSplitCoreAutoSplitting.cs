/// API Docs: https://github.com/LiveSplit/livesplit-core/tree/master/crates/livesplit-auto-splitting#requirements-for-the-auto-splitters
namespace LiveSplitCore.AutoSplitting {
    
    [UnmanagedCallersOnly(EntryPoint = "update")]
    static void update() {}
    
    using usize = System.Uint64;
    // TODO: how to handle these?
    using NonZeroU64 = System.Uint64;
    using OptionNonZeroU64 = System.Uint64;
    using OptionProcessId = System.Uint64;
    using OptionNonZeroAddress = System.Uint64;

    [StructLayout(LayoutKind.Sequential)]
    static extern struct Address { System.Uint64 Value; };

    [StructLayout(LayoutKind.Sequential)]
    static extern struct NonZeroAddress { NonZeroU64 Value; };

    [StructLayout(LayoutKind.Sequential)]
    static extern struct ProcessId { NonZeroU64 Value; };

    [StructLayout(LayoutKind.Sequential)]
    static extern struct TimerState {
        System.Uint32 Value;

        /// The timer is not running.
        static const TimerState NOT_RUNNING = TimerState(0);
        /// The timer is running.
        static const TimerState RUNNING = TimerState(1);
        /// The timer started but got paused. This is separate from the game
        /// time being paused. Game time may even always be paused.
        static const TimerState PAUSED = TimerState(2);
        /// The timer has ended, but didn't get reset yet.
        static const TimerState ENDED = TimerState(3);
    };

    /// Gets the state that the timer currently is in.
    [DllImport("*", EntryPoint = "timer_get_state")]
    static extern TimerState timer_get_state();

    /// Starts the timer.
    [DllImport("*", EntryPoint = "timer_start")]
    static extern void timer_start();
    /// Splits the current segment.
    [DllImport("*", EntryPoint = "timer_split")]
    static extern void timer_split();
    /// Resets the timer.
    [DllImport("*", EntryPoint = "timer_reset")]
    static extern void timer_reset();
    /// Sets a custom key value pair. This may be arbitrary information that
    /// the auto splitter wants to provide for visualization.
    [DllImport("*", EntryPoint = "timer_set_variable")]
    static extern void timer_set_variable(
        byte* key_ptr,
        usize key_len,
        byte* value_ptr,
        usize value_len
    );

    /// Sets the game time.
    [DllImport("*", EntryPoint = "timer_set_game_time")]
    static extern void timer_set_game_time(System.Int64 secs, System.Int32 nanos);
    /// Pauses the game time. This does not pause the timer, only the
    /// automatic flow of time for the game time.
    [DllImport("*", EntryPoint = "timer_pause_game_time")]
    static extern void timer_pause_game_time();
    /// Resumes the game time. This does not resume the timer, only the
    /// automatic flow of time for the game time.
    [DllImport("*", EntryPoint = "timer_resume_game_time")]
    static extern void timer_resume_game_time();

    /// Attaches to a process based on its name.
    [DllImport("*", EntryPoint = "process_attach")]
    static extern OptionProcessId process_attach(byte* name_ptr, usize name_len);
    /// Detaches from a process.
    [DllImport("*", EntryPoint = "process_detach")]
    static extern void process_detach(ProcessId process);
    /// Checks whether is a process is still open. You should detach from a
    /// process and stop using it if this returns `false`.
    [DllImport("*", EntryPoint = "process_is_open")]
    static extern bool process_is_open(ProcessId process);
    /// Reads memory from a process at the address given. This will write
    /// the memory to the buffer given. Returns `false` if this fails.
    [DllImport("*", EntryPoint = "process_read")]
    static extern bool process_read(
        ProcessId process,
        Address address,
        out byte* buf_ptr,
        usize buf_len
    );
    /// Gets the address of a module in a process.
    [DllImport("*", EntryPoint = "process_get_module_address")]
    static extern OptionNonZeroAddress process_get_module_address(
        ProcessId process,
        byte* name_ptr,
        usize name_len
    );
    /// Gets the size of a module in a process.
    [DllImport("*", EntryPoint = "process_get_module_size")]
    static extern OptionNonZeroU64 process_get_module_size(
        ProcessId process,
        byte* name_ptr,
        usize name_len
    );

    /// Sets the tick rate of the runtime. This influences the amount of
    /// times the `update` function is called per second.
    [DllImport("*", EntryPoint = "runtime_set_tick_rate")]
    static extern void runtime_set_tick_rate(double ticks_per_second);
    /// Prints a log message for debugging purposes.
    [DllImport("*", EntryPoint = "runtime_print_message")]
    static extern void runtime_print_message(byte* text_ptr, usize text_len);

    /// Adds a new setting that the user can modify. This will return either
    /// the specified default value or the value that the user has set.
    [DllImport("*", EntryPoint = "user_settings_add_bool")]
    static extern bool user_settings_add_bool(
        byte* key_ptr,
        usize key_len,
        byte* description_ptr,
        usize description_len,
        bool default_value
    );
}