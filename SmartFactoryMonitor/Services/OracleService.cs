using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFactoryMonitor.Services
{
    public class OracleService
    {
        private readonly string ConnString = "Password=hhong;Persist Security Info=True;User ID=hhong;Data Source=DEVBEAN";

        public bool OpenOracle()
        {
            using (OracleConnection conn = new OracleConnection(ConnString))
            {
                try
                {
                    conn.Open();
                    return true;
                }
                catch { return false; }
            }
        }

        public async Task<DataTable> SelectQuery(string sql)
        {
            // using 블록을 사용하면 close()를 명시적으로 호출 x
            using (OracleConnection conn = new OracleConnection(ConnString))
            {
                DataTable dt = new DataTable();
                try
                {
                    // 1. 연결을 오픈한다
                    await conn.OpenAsync();

                    // 2. 커맨드를 생성한다
                    using (OracleCommand cmd = conn.CreateCommand())
                    {
                        // 3. 트랜잭션을 시작한다
                        using (OracleTransaction transaction = conn.BeginTransaction(IsolationLevel.ReadCommitted))
                        {
                            // 4. 커맨드(오라클 트랜잭션)을 생성한 트랜잭션에 연결한다
                            cmd.Transaction = transaction;

                            // 5. 커맨드에 sql 삽입 및 수행
                            cmd.CommandText = sql;
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                dt.Load(reader);
                            }
                            // 조회 작업이므로 커밋 x 
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

                return dt;
            }
        }

        public async Task<string> ExecuteQuery(string sql)
        {
            string result = string.Empty;
            using (OracleConnection conn = new OracleConnection(ConnString))
            {
                try
                {
                    // 1. 연결을 오픈한다
                    await conn.OpenAsync();

                    // 2. 커맨드를 생성한다
                    using (OracleCommand cmd = conn.CreateCommand())
                    {
                        // 3. 트랜잭션을 시작한다
                        using (OracleTransaction transaction = conn.BeginTransaction(IsolationLevel.ReadCommitted))
                        {
                            try
                            {
                                // 4. 커맨드(오라클 트랜잭션)을 생성한 트랜잭션에 연결한다
                                cmd.Transaction = transaction;

                                // 5. 커맨드에 sql 삽입
                                cmd.CommandText = sql;

                                // 6. 커맨드 수행
                                result = (await cmd.ExecuteNonQueryAsync()).ToString();

                                // 7. 커맨드 결과 커밋
                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                result = ex.Message;
                                transaction.Rollback(); // 오류 발생 시 롤백
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                    Console.WriteLine($"Error: {ex.Message}");
                }

                return result;
            }
        }

        //    public DataTable SelectQuery(string sql)
        //    {
        //        // using 블록을 사용하면 close()를 명시적으로 호출 x
        //        using (OracleConnection conn = new OracleConnection(ConnString))
        //        {
        //            DataTable dt = new DataTable();
        //            try
        //            {
        //                // 1. 연결을 오픈한다
        //                conn.Open();

        //                // 2. 커맨드를 생성한다
        //                using (OracleCommand cmd = conn.CreateCommand())
        //                {
        //                    // 3. 트랜잭션을 시작한다
        //                    using (OracleTransaction transaction = conn.BeginTransaction(IsolationLevel.ReadCommitted))
        //                    {
        //                        // 4. 커맨드(오라클 트랜잭션)을 생성한 트랜잭션에 연결한다
        //                        cmd.Transaction = transaction;

        //                        // 5. 커맨드에 sql 삽입 및 수행
        //                        cmd.CommandText = sql;
        //                        using (OracleDataReader reader = cmd.ExecuteReader())
        //                        {
        //                            dt.Load(reader);
        //                        }
        //                        // 조회 작업이므로 커밋 x 
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine($"Error: {ex.Message}");
        //            }

        //            return dt;
        //        }
        //    }

        //    public string ExecuteQuery(string sql)
        //    {
        //        string result = string.Empty;
        //        // using 블록을 사용하면 close()를 명시적으로 호출 x
        //        using (OracleConnection conn = new OracleConnection(ConnString))
        //        {
        //            DataTable dt = new DataTable();
        //            try
        //            {
        //                // 1. 연결을 오픈한다
        //                conn.Open();

        //                // 2. 커맨드를 생성한다
        //                using (OracleCommand cmd = conn.CreateCommand())
        //                {
        //                    // 3. 트랜잭션을 시작한다
        //                    using (OracleTransaction transaction = conn.BeginTransaction(IsolationLevel.ReadCommitted))
        //                    {
        //                        try
        //                        {
        //                            // 4. 커맨드(오라클 트랜잭션)을 생성한 트랜잭션에 연결한다
        //                            cmd.Transaction = transaction;

        //                            // 5. 커맨드에 sql 삽입
        //                            cmd.CommandText = sql;

        //                            // 6. 커맨드 수행
        //                            result = cmd.ExecuteNonQuery().ToString();

        //                            // 7. 커맨드 결과 커밋
        //                            transaction.Commit();
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            result = ex.Message;
        //                            transaction.Rollback(); // 오류 발생 시 롤백
        //                        }
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                result = ex.Message;
        //                Console.WriteLine($"Error: {ex.Message}");
        //            }

        //            return result;
        //        }
        //    }
    }
}
