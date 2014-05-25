using System;
using System.Linq;
using MyNotifyIcon;
using NUnit.Framework;

namespace MyNotifyIconUnitTests
{
    [TestFixture]
    public class PositionQueueTests
    {
        protected const double _origX = 64;
        protected const double _origY = 1200;
        protected const double _offsetY = 120;
        private PositionQueue _queue;

        [SetUp]
        public void Arrange()
        {
            _queue = new PositionQueue(new Point() { X = (int)_origX, Y = (int)_origY }, _offsetY);
        }

        [TestFixture]
        public class Given_empty_position_queue : PositionQueueTests
        {
            private Position _pos;

            [SetUp]
            public void Arrange()
            {// empty by purpose 
            }

            [TestFixture]
            public class When_release_position : Given_empty_position_queue
            {
                [SetUp]
                public void Act()
                {
                    _queue.ReleasePositionAndClearPositionsIfAllAreFree("");
                }

                [Test]
                public void queue_length_equal_to_0()
                {
                    Assert.AreEqual(0, _queue.Count);
                }
            }

            [TestFixture]
            public class When_obtain_position : Given_empty_position_queue
            {
                [SetUp]
                public void Act()
                {
                    _pos = _queue.OccupyPosition("1");
                }

                [Test]
                public void length_of_position_queue_equal_to_1()
                {
                    Assert.AreEqual(1, _queue.Count);
                }

                [Test]
                public void position_offsetX_equal_to_origOffsetX()
                {
                    Assert.AreEqual(_origX, _pos.OffsetX);
                }

                [Test]
                public void position_offsetY_equal_to_origOffsetY()
                {
                    Assert.AreEqual(_origY, _pos.OffsetY);
                }

                [Test]
                public void position_is_occupied()
                {
                    Assert.IsTrue(_pos.IsOccupied);
                }

                [Test]
                public void position_id_equal_to_1()
                {
                    Assert.AreEqual("1", _pos.Id);
                }
            }

            [TestFixture]
            public class When_obtain_position_twice : Given_empty_position_queue
            {
                [SetUp]
                public void Act()
                {
                    _pos = _queue.OccupyPosition("1");
                    _pos = _queue.OccupyPosition("2");
                }

                [Test]
                public void length_of_position_queue_equal_to_2()
                {
                    Assert.AreEqual(2, _queue.Count);
                }

                [Test]
                public void position_offsetX_equal_to_origOffsetX()
                {
                    Assert.AreEqual(_origX, _pos.OffsetX);
                }

                [Test]
                public void position_offsetY_equal_to_origOffsetY_minus_offsetY()
                {
                    Assert.AreEqual(_origY - _offsetY, _pos.OffsetY);
                }

                [Test]
                public void position_is_occupied()
                {
                    Assert.IsTrue(_pos.IsOccupied);
                }

                [Test]
                public void position_id_equal_to_2()
                {
                    Assert.AreEqual("2", _pos.Id);
                }
            }

            [TestFixture]
            public class When_obtain_position_third_times : Given_empty_position_queue
            {
                [SetUp]
                public void Act()
                {
                    _pos = _queue.OccupyPosition("1");
                    _pos = _queue.OccupyPosition("2");
                    _pos = _queue.OccupyPosition("3");
                }

                [Test]
                public void position_offsetY_equal_to_origOffsetY_minus_double_offsetY()
                {
                    Assert.AreEqual(_origY - _offsetY * 2, _pos.OffsetY);
                }
            }
        }

        [TestFixture]
        public class Given_3_occupied_positions_in_queue : PositionQueueTests
        {
            private Position _pos;

            [SetUp]
            public void Arange()
            {
                _queue.Positions.Add(new Position { Id = "1", IsOccupied = true, OffsetX = _origX, OffsetY = _offsetY });
                _queue.Positions.Add(new Position { Id = "2", IsOccupied = true, OffsetX = _origX, OffsetY = _origY - _offsetY });
                _queue.Positions.Add(new Position { Id = "3", IsOccupied = true, OffsetX = _origX, OffsetY = _origY - 2 * _offsetY });
            }

            [TestFixture]
            public class When_release_first_position : Given_3_occupied_positions_in_queue
            {
                [SetUp]
                public void Act()
                {
                    _queue.ReleasePositionAndClearPositionsIfAllAreFree("1");
                }

                [Test]
                public void queue_length_equal_to_3()
                {
                    Assert.AreEqual(3, _queue.Count);
                }

                [Test]
                public void first_position_not_occupied()
                {
                    Assert.IsFalse(_queue.Positions[0].IsOccupied);
                }

                [Test]
                public void first_position_id_is_empty()
                {
                    Assert.IsTrue(String.IsNullOrEmpty(_queue.Positions[0].Id));
                }

                [Test]
                public void second_position_occupied()
                {
                    Assert.IsTrue(_queue.Positions[1].IsOccupied);
                }

                [Test]
                public void third_position_occupied()
                {
                    Assert.IsTrue(_queue.Positions[2].IsOccupied);
                }
            }

            [TestFixture]
            public class When_release_second_position : Given_3_occupied_positions_in_queue
            {
                [SetUp]
                public void Act()
                {
                    _queue.ReleasePositionAndClearPositionsIfAllAreFree("2");
                }

                [Test]
                public void queue_length_equal_to_3()
                {
                    Assert.AreEqual(3, _queue.Count);
                }

                [Test]
                public void first_position_occupied()
                {
                    Assert.IsTrue(_queue.Positions[0].IsOccupied);
                }

                [Test]
                public void second_position_not_occupied()
                {
                    Assert.IsFalse(_queue.Positions[1].IsOccupied);
                }

                [Test]
                public void second_position_id_is_empty()
                {
                    Assert.IsTrue(String.IsNullOrEmpty(_queue.Positions[1].Id));
                }

                [Test]
                public void third_position_occupied()
                {
                    Assert.IsTrue(_queue.Positions[2].IsOccupied);
                }
            }

            [TestFixture]
            public class When_release_all_positions : Given_3_occupied_positions_in_queue
            {
                [SetUp]
                public void Act()
                {
                    _queue.ReleasePositionAndClearPositionsIfAllAreFree("1");
                    _queue.ReleasePositionAndClearPositionsIfAllAreFree("2");
                    _queue.ReleasePositionAndClearPositionsIfAllAreFree("3");
                }

                [Test]
                public void queue_length_equal_to_0()
                {
                    Assert.AreEqual(0, _queue.Count);
                }
            }
        }

        [TestFixture]
        public class Given_3_positions_and_second_not_occupied : PositionQueueTests
        {
            private Position _pos;

            [SetUp]
            public void Arrange()
            {
                _queue.Positions.Add(new Position { Id = "1", IsOccupied = true, OffsetX = _origX, OffsetY = _origY });
                _queue.Positions.Add(new Position { Id = "", IsOccupied = false, OffsetX = _origX, OffsetY = _origY - _offsetY });
                _queue.Positions.Add(new Position { Id = "3", IsOccupied = true, OffsetX = _origX, OffsetY = _origY - 2 * _offsetY });
            }

            [TestFixture]
            public class When_obtain_position : Given_3_positions_and_second_not_occupied
            {
                [SetUp]
                public void Act()
                {
                    _pos = _queue.OccupyPosition("999");
                }

                [Test]
                public void queue_length_equal_to_3()
                {
                    Assert.AreEqual(3, _queue.Count);
                }

                [Test]
                public void position_id_equal_to_999()
                {
                    Assert.AreEqual("999", _pos.Id);
                }

                [Test]
                public void position_offsetX_equal_to_origOffsetX()
                {
                    Assert.AreEqual(_pos.OffsetX, _origX);
                }

                [Test]
                public void position_offsetY_equal_to_origOffsetY_minus_offsetY()
                {
                    Assert.AreEqual(_pos.OffsetY, _origY - _offsetY);
                }
            }
        }
    }
}
