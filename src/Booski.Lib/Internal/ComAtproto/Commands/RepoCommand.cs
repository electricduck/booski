using Booski.Lib.Common;
using Booski.Lib.Internal.ComAtproto.Requests;
using Booski.Lib.Internal.ComAtproto.Responses;
using Booski.Lib.Xrpc;
using ComAtprotoConstants = Booski.Lib.Internal.ComAtproto.Common.Constants;

namespace Booski.Lib.Internal.ComAtproto.Commands {
    public class RepoCommand : Com.Atproto.Repo
    {
        private readonly AtProto _atProto;

        public RepoCommand(
            AtProto atProto
        ) {
            _atProto = atProto;
        }

        public async Task<AtProtoApiResponse<ApplyWritesResponse>> ApplyWrites(
            string repo,
            List<Lexicon> writes,
            string swapCommit = "",
            bool validate = true
        ) {
            ApplyWritesRequest applyWritesRequest = new ApplyWritesRequest {
                Repo = repo,
                SwapCommit = swapCommit,
                Validate = validate,
                Writes = writes
            };

            return await _atProto.PostJson<ApplyWritesResponse, ApplyWritesRequest>(
                lexicon: "com.atproto.repo.applyWrites",
                body: applyWritesRequest
            );
        }

        public async Task<AtProtoApiResponse<CreateRecordResponse>> CreateRecord(
            string collection,
            dynamic record,
            string repo,
            string rKey = "",
            string swapCommit = "",
            bool validate = true
        ) {
            CreateRecordRequest createRecordRequest = new CreateRecordRequest {
                Collection = collection,
                Record = record,
                Repo = repo,
                RKey = rKey,
                SwapCommit = (String.IsNullOrEmpty(swapCommit) ? null : swapCommit),
                Validate = validate
            };

            return await _atProto.PostJson<CreateRecordResponse, CreateRecordRequest>(
                lexicon: "com.atproto.repo.createRecord",
                body: createRecordRequest
            );
        }

        public async Task<AtProtoApiResponse<DeleteRecordResponse>> DeleteRecord(
            string collection,
            string repo,
            string rKey = "",
            string swapCommit = "",
            string swapRecord = ""
        ) {
            DeleteRecordRequest deleteRecordRequest = new DeleteRecordRequest {
                Collection = collection,
                Repo = repo,
                RKey = rKey,
                SwapCommit = (String.IsNullOrEmpty(swapCommit) ? null : swapCommit),
                SwapRecord = (String.IsNullOrEmpty(swapRecord) ? null : swapRecord)
            };

            return await _atProto.PostJson<DeleteRecordResponse, DeleteRecordRequest>(
                lexicon: "com.atproto.repo.deleteRecord",
                body: deleteRecordRequest
            );
        }

        public async Task<AtProtoApiResponse<DescribeRepoResponse>> DescribeRepo(
            string repo
        ) {
            List<QueryParam> describeRepoParams = new List<QueryParam> {
                new QueryParam("repo", repo)
            };

            return await _atProto.GetJson<DescribeRepoResponse>(
                lexicon: "com.atproto.repo.describeRepo",
                queries: describeRepoParams
            );
        }

        public async Task<AtProtoApiResponse<GetRecordResponse>> GetRecord(
            string collection,
            string repo,
            string rKey,
            string cid = ""
        ) {
            List<QueryParam> getRecordParams = new List<QueryParam> {
                new QueryParam("cid", cid),
                new QueryParam("collection", collection),
                new QueryParam("repo", repo),
                new QueryParam("rkey", rKey)
            };

            return await _atProto.GetJson<GetRecordResponse>(
                lexicon: "com.atproto.repo.getRecord",
                queries: getRecordParams
            );
        }

        public async Task<AtProtoApiResponse<ImportRepoResponse>> ImportRepo(
            byte[] blob
        )
        {
            // TODO: Verify CAR file

            return await _atProto.PostBlob<ImportRepoResponse>(
                lexicon: "com.atproto.repo.importRepo",
                data: blob,
                mimeType: "application/vnd.ipld.car"
            );
        }

        public async Task<AtProtoApiResponse<ImportRepoResponse>> ImportRepo(
            string path
        )
        {
            byte[] bytes = GetFileBytes(path);
            return await ImportRepo(bytes);
        }

        public async Task<AtProtoApiResponse<ListMissingBlobsReponse>> ListMissingBlobs(
            string cursor = "",
            int limit = 500
        )
        {
            List<QueryParam> listMissingBlobsParams = new List<QueryParam> {
                new QueryParam("cursor", cursor),
                new QueryParam("limit", limit.ToString())
            };

            return await _atProto.GetJson<ListMissingBlobsReponse>(
                lexicon: "com.atproto.repo.listMissingBlobs",
                queries: listMissingBlobsParams
            );
        }

        public async Task<AtProtoApiResponse<ListRecordsResponse>> ListRecords(
            string collection,
            string repo,
            string cursor = "",
            int limit = 50,
            bool reverse = false
        )
        {
            List<QueryParam> listRecordsParams = new List<QueryParam> {
                new QueryParam("collection", collection),
                new QueryParam("cursor", cursor),
                new QueryParam("limit", limit.ToString()),
                new QueryParam("repo", repo),
                new QueryParam("reverse", reverse.ToString().ToLower()),
            };

            return await _atProto.GetJson<ListRecordsResponse>(
                lexicon: "com.atproto.repo.listRecords",
                queries: listRecordsParams
            );
        }

        public async Task<AtProtoApiResponse<PutRecordResponse>> PutRecord(
            string collection,
            dynamic record,
            string repo,
            string rKey,
            string swapCommit = "",
            string swapRecord = "",
            bool validate = true
        ) {
            PutRecordRequest putRecordRequest = new PutRecordRequest {
                Collection = collection,
                Record = record,
                Repo = repo,
                RKey = rKey,
                SwapCommit = (String.IsNullOrEmpty(swapCommit) ? null : swapCommit),
                SwapRecord = (String.IsNullOrEmpty(swapRecord) ? null : swapRecord),
                Validate = validate
            };

            return await _atProto.PostJson<PutRecordResponse, PutRecordRequest>(
                lexicon: "com.atproto.repo.putRecord",
                body: putRecordRequest
            );
        }

        public async Task<AtProtoApiResponse<UploadBlobResponse>> UploadBlob(
            byte[] blob
        )
        {
            return await _atProto.PostBlob<UploadBlobResponse>(
                lexicon: "com.atproto.repo.uploadBlob",
                data: blob
            );
        }

        public async Task<AtProtoApiResponse<UploadBlobResponse>> UploadBlob(
            string path
        )
        {
            byte[] bytes = GetFileBytes(path);
            return await UploadBlob(bytes);
        }

        private byte[] GetFileBytes(string path) {
            if(!System.IO.File.Exists(path))
                throw new FileNotFoundException($"File '{path}' does not exist.");

            return System.IO.File.ReadAllBytes(path);
        }
    }
}