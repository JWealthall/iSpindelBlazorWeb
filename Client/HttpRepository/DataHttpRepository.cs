using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using iSpindelBlazorWeb.Shared.Models;

namespace iSpindelBlazorWeb.Client.HttpRepository
{
    public interface IDataHttpRepository
    {
        Task<Batch> CreateBatch(Batch batch);
        Task<Log> DeleteLog(Log log);
        Task<Batch> EndBatch(Batch batch);
        Task<Batch> GetBatch(Guid id);
        Task<SummaryDataModel> GetBatchesSummary();
        Task<SummaryDataModel> GetBatchSummary(Guid id);
        Task<Device> GetDevice(Guid id);
        Task<SummaryDataModel> GetDevicesSummary();
        Task<SummaryDataModel> GetDeviceSummary(Guid id);
        Task<Log> GetLog(Guid id);
        Task<SummaryDataModel> GetSummary();
        Task<Batch> UpdateBatch(Batch batch);
        Task<Device> UpdateDevice(Device device);
    }

    // This is a basic mechanism for pulling back data.
    // It does limited caching which should be expanded upon in a new system
    // TODO:
    //  - Better error handling
    //  - Authorization (where required)
    public class DataHttpRepository : IDataHttpRepository
    {
        // NOTE: All of the methods in here could be set up to only got back to the server if getting data for a different device/batch
        // or if the existing device batch wasn't loaded within a certain timeframe (as we know updates are performed through this app/api then we could detect that)
        private static DateTime _lastUpdated = DateTime.MinValue;
        private static SummaryDataModel _lastBatchesSummary;
        private static Guid _lastBatchId = Guid.Empty;
        private static SummaryDataModel _lastBatchSummary;
        private static SummaryDataModel _lastDevicesSummary;
        private static Guid _lastDeviceId = Guid.Empty;
        private static SummaryDataModel _lastDeviceSummary;
        private static SummaryDataModel _lastSummary;

        private readonly HttpClient _client;
        public DataHttpRepository(HttpClient client)
        {
            _client = client;
        }

        public async Task<Batch> CreateBatch(Batch batch)
        {
            var res = await _client.PostAsJsonAsync<Batch>("Data/BatchCreate", batch);
            var devRes = res.Content.ReadFromJsonAsync<Batch>().Result;
            if (devRes != null && string.IsNullOrWhiteSpace(devRes.StatusData.Message) && devRes.BatchId == _lastBatchId)
            {
                _lastBatchSummary = null;
                _lastDeviceSummary = null;
                _lastSummary = null;
            }
            return devRes;
        }

        public async Task<Log> DeleteLog(Log log)
        {
            var res = await _client.PostAsJsonAsync<Log>($"Data/LogDelete/{log.LogId}", log);
            var devRes = res.Content.ReadFromJsonAsync<Log>().Result;
            if (devRes != null && string.IsNullOrWhiteSpace(devRes.StatusData.Message) && devRes.BatchId == _lastBatchId)
            {
                _lastBatchSummary = null;
                _lastDeviceSummary = null;
                _lastSummary = null;
            }
            return devRes;
        }

        public async Task<Batch> EndBatch(Batch batch)
        {
            var res = await _client.PostAsJsonAsync<Batch>($"Data/BatchEnd/{batch.BatchId}", batch);
            var devRes = res.Content.ReadFromJsonAsync<Batch>().Result;
            if (devRes != null && string.IsNullOrWhiteSpace(devRes.StatusData.Message) && devRes.BatchId == _lastBatchId)
            {
                _lastBatchSummary = null;
                _lastDeviceSummary = null;
                _lastSummary = null;
            }
            return devRes;
        }

        private async Task<DateTime> GetLastUpdated()
        {
            return await _client.GetFromJsonAsync<DateTime>("Data/LastUpdated");
        }

        public async Task<Batch> GetBatch(Guid id)
        {
            return await _client.GetFromJsonAsync<Batch>($"Data/Batch/{id}");
        }

        public async Task<SummaryDataModel> GetBatchesSummary()
        {
            if (_lastBatchesSummary != null)
            {
                var newLastUpdated = await GetLastUpdated();
                if (newLastUpdated <= _lastUpdated) return _lastBatchesSummary;
            }
            _lastBatchesSummary = await _client.GetFromJsonAsync<SummaryDataModel>("Data/BatchesSummary");
            if (_lastBatchesSummary != null) _lastUpdated = _lastBatchesSummary.LastUpdated;
            return _lastBatchesSummary;
        }

        public async Task<SummaryDataModel> GetBatchSummary(Guid id)
        {
            if (_lastBatchSummary != null && _lastBatchId == id)
            {
                var newLastUpdated = await GetLastUpdated();
                if (newLastUpdated <= _lastUpdated) return _lastBatchSummary;
            }
            _lastBatchSummary = await _client.GetFromJsonAsync<SummaryDataModel>($"Data/BatchSummary/{id}");
            if (_lastBatchSummary != null) _lastUpdated = _lastBatchSummary.LastUpdated;
            _lastBatchId = id;
            return _lastBatchSummary;
        }

        public async Task<Device> GetDevice(Guid id)
        {
            // TODO: Detect if an "empty" device was returned
            try
            {
                return await _client.GetFromJsonAsync<Device>($"Data/Device/{id}");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("Http:" + e);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<SummaryDataModel> GetDevicesSummary()
        {
            if (_lastDevicesSummary != null)
            {
                var newLastUpdated = await GetLastUpdated();
                if (newLastUpdated <= _lastUpdated) return _lastDevicesSummary;
            }
            _lastDevicesSummary = await _client.GetFromJsonAsync<SummaryDataModel>("Data/DevicesSummary");
            if (_lastDevicesSummary != null) _lastUpdated = _lastDevicesSummary.LastUpdated;
            return _lastDevicesSummary;
        }

        public async Task<SummaryDataModel> GetDeviceSummary(Guid id)
        {
            if (_lastDeviceSummary != null && _lastDeviceId == id)
            {
                var newLastUpdated = await GetLastUpdated();
                if (newLastUpdated <= _lastUpdated) return _lastDeviceSummary;
            }
            _lastDeviceSummary = await _client.GetFromJsonAsync<SummaryDataModel>($"Data/DeviceSummary/{id}");
            if (_lastDeviceSummary != null) _lastUpdated = _lastDeviceSummary.LastUpdated;
            _lastDeviceId = id;
            return _lastDeviceSummary;
        }

        public async Task<Log> GetLog(Guid id)
        {
            return await _client.GetFromJsonAsync<Log>($"Data/Log/{id}");
        }

        public async Task<SummaryDataModel> GetSummary()
        {
            if (_lastSummary != null)
            {
                var newLastUpdated = await GetLastUpdated();
                if (newLastUpdated <= _lastUpdated) return _lastSummary;
            }
            _lastSummary = await _client.GetFromJsonAsync<SummaryDataModel>("Data/Summary");
            if (_lastSummary != null) _lastUpdated = _lastSummary.LastUpdated;
            return _lastSummary;
        }

        public async Task<Batch> UpdateBatch(Batch batch)
        {
            var res = await _client.PostAsJsonAsync<Batch>($"Data/BatchUpdate/{batch.BatchId}", batch);
            var devRes = res.Content.ReadFromJsonAsync<Batch>().Result;
            if (devRes != null && string.IsNullOrWhiteSpace(devRes.StatusData.Message) && devRes.BatchId == _lastBatchId)
            {
                _lastBatchSummary = null;
                _lastDeviceSummary = null;
                _lastSummary = null;
            }
            return devRes;
        }

        public async Task<Device> UpdateDevice(Device device)
        {
            var res = await _client.PostAsJsonAsync<Device>($"Data/DeviceUpdate/{device.DeviceId}", device);
            var devRes = res.Content.ReadFromJsonAsync<Device>().Result;
            if (devRes != null && string.IsNullOrWhiteSpace(devRes.StatusData.Message) && devRes.DeviceId == _lastDeviceId) _lastDeviceSummary = null;
            return devRes;
        }

    }
}
