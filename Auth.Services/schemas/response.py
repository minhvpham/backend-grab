from pydantic import BaseModel
from typing import Optional

class APIResponse(BaseModel):
    code: int
    message: str
    data: Optional[str] = None
    error: Optional[str] = None
