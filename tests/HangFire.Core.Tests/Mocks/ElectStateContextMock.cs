﻿using System;
using HangFire.States;
using HangFire.Storage;
using Moq;

namespace HangFire.Core.Tests
{
    public class ElectStateContextMock
    {
        private readonly Lazy<ElectStateContext> _context;

        public ElectStateContextMock()
        {
            StateContextValue = new StateContextMock();
            CandidateStateValue = new Mock<IState>().Object;
            CurrentStateValue = "OldState";

            _context = new Lazy<ElectStateContext>(
                () => new ElectStateContext(
                    StateContextValue.Object, 
                    CandidateStateValue,
                    CurrentStateValue));
        }

        public StateContextMock StateContextValue { get; set; }
        public IState CandidateStateValue { get; set; }
        public string CurrentStateValue { get; set; }

        public ElectStateContext Object
        {
            get { return _context.Value; }
        }
    }
}
