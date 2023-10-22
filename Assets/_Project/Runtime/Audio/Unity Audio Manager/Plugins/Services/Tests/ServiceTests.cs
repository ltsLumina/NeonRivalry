using FLZ.Services;
using NUnit.Framework;

namespace FLZ.Tests
{
    public interface IMockService : IService { }
    
    public abstract class BaseMockService : IMockService
    {
        public void OnPreAwake() { }

        public void OnAfterAwake() { }

        public bool IsReady()
        {
            return true;
        }
    }
    
    public class MockServiceA : BaseMockService { }
    public class MockServiceB : BaseMockService { }
    
    public class ServiceTests
    {
        [SetUp]
        public void Init() { }

        [TearDown]
        public void TearDown()
        {
            ServiceProvider.Dispose();
        }
        
        [Test]
        public void ServiceProvider_GetService_ReturnService()
        {
            ServiceProvider.AddService(typeof(MockServiceA), new MockServiceA());

            var service = ServiceProvider.GetService<MockServiceA>();
            Assert.IsNotNull(service);
        }
        
        [Test]
        public void ServiceProvider_GetServices_ReturnServices()
        {
            ServiceProvider.AddService(typeof(IMockService), new MockServiceA());
            ServiceProvider.AddService(typeof(IMockService), new MockServiceB());

            var services = ServiceProvider.GetServices<IMockService>();
            
            Assert.IsTrue(services.Count == 2);
        }

        [Test]
        public void ServiceProvider_RemoveServiceByInterface_RemoveService()
        {
            ServiceProvider.AddService(typeof(IMockService), new MockServiceA());
            ServiceProvider.AddService(typeof(IMockService), new MockServiceB());

            ServiceProvider.RemoveService<IMockService>();

            var service = ServiceProvider.GetService<IMockService>();
            Assert.IsNull(service);
        }
        
        [Test]
        public void ServiceProvider_DuplicatedService_ThrowException()
        {
            Assert.That( () =>
            {
                ServiceProvider.AddService(typeof(IMockService), new MockServiceA());
                ServiceProvider.AddService(typeof(IMockService), new MockServiceA());
                
            }, Throws.Exception);
        }


        [Test]
        public void ServiceRef_GetByType_ReturnService()
        {
            ServiceProvider.AddService(typeof(MockServiceA), new MockServiceA());

            var service = new ServiceRef<MockServiceA>().Value;
            Assert.IsNotNull(service);
        }
        
        [Test]
        public void ServiceRef_GetByInterfaceType_ReturnService()
        {
            ServiceProvider.AddService(typeof(IMockService), new MockServiceA());

            var service = new ServiceRef<IMockService, MockServiceA>().Value;
            Assert.IsNotNull(service);
        }
    }
}