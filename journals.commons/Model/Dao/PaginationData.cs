namespace journals.commons.Model.dao {
    public class PaginationData {

        public PaginationData() {
            PageSize = 10;
            PageNumber = 1;
        }

        public int PageSize {
            get; set;
        }
        public int PageNumber {
            get; set;
        }
        //this is needed because nhibernate builds buggy pagination queries, not appending the the alias on the over query. 
        //in some scenarios this may lead to ambigous column definition
        public string QualifiedOrderByColumn {
            get; set;
        }
        public string OrderByColumn {
            get; set;
        }
    }
}