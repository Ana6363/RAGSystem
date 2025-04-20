import os
from langchain.chains.question_answering import load_qa_chain
from langchain_openai import OpenAI
from dotenv import load_dotenv
from langchain_community.vectorstores import Qdrant
from langchain_openai import OpenAIEmbeddings
from langchain_qdrant import Qdrant
from qdrant_client import QdrantClient

load_dotenv()

OPENAI_API_KEY = os.getenv("OPENAI_API_KEY")
QDRANT_URL = os.getenv("QDRANT_URL")
QDRANT_API_KEY = os.getenv("QDRANT_API_KEY")
QDRANT_COLLECTION = os.getenv("QDRANT_COLLECTION", "nbanks_documents")

embeddings = OpenAIEmbeddings(api_key=OPENAI_API_KEY)

# Official client from the Qdrant Python SDK
# This is used to connect to Qdrant Cloud (url) and authenticate (api_key)
qdrant_client = QdrantClient(
    url=QDRANT_URL,
    api_key=QDRANT_API_KEY,
)

# LanchChain's Qdrant wrapper
# Plugs Qdrant into its vector retrieval system
# It will know which collection to search (collection_name)
# Which embeddings to use (embeddings)
# Where to send the search request (client)
vectorstore = Qdrant(
    client=qdrant_client,
    collection_name=QDRANT_COLLECTION,
    embeddings=embeddings,
)

llm = OpenAI(temperature=0, openai_api_key=OPENAI_API_KEY)


def query_pdf(file_id: str, question: str) -> str:
    
    # Semantic Search in Qdrant
    relevant_docs = vectorstore.similarity_search(
        query=question,
        k=4,
        filter={"file_id": file_id}
    )

    if not relevant_docs:
        raise ValueError(f"No chunks found for file_id: {file_id}")

    chain = load_qa_chain(llm, chain_type="stuff")
    result = chain.run(input_documents=relevant_docs, question=question)
    return result