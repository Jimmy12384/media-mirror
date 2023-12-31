using System;
using System.Windows;

namespace MediaMirror {
    class IdleState : AbsState {
        public IdleState(MediaMirrorContext ctx, AbsState parent) : base(ctx, parent) { }

        public override void AddMusicButtonClick(object sender, RoutedEventArgs e) {
            ctx.CurrentState = new HostState(ctx, this);
            ctx.CurrentState.Enter();
        }

        public override void ConnectButtonClick(object sender, RoutedEventArgs e) {
            ctx.CurrentState = new ClientState(ctx, this);
            ctx.CurrentState.Enter();
        }

        public override void Enter() {
            Console.WriteLine("Idle State Entered");
        }
    }
}
